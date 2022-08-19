using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

/// <summary>
/// Handles everything related to the collider of the character. This is actually an empty game object, NOT on the character prefab
/// as for gameplay reason, we need a single size collider for every character. (Found on the Main scene PlayerPivot/CharacterSlot gameobject)
/// Xử lý mọi thứ liên quan đến máy va chạm của nhân vật. Đây thực sự là một đối tượng trò chơi trống rỗng, KHÔNG có trên bản cài sẵn nhân vật
/// vì lý do trò chơi, chúng tôi cần một máy va chạm kích thước duy nhất cho mọi nhân vật. (Tìm thấy trên trò chơi PlayerPivot / CharacterSlot của cảnh chính)
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class CharacterCollider : MonoBehaviour
{
	static int s_HitHash = Animator.StringToHash("Hit");
    //giá trị nhất nháy
    static int s_BlinkingValueHash;

	// Used mainly by by analytics, but not in an analytics ifdef block 
	// so that the data is available to anything (e.g. could be used for player stat saved locally etc.)
	// Được sử dụng chủ yếu bởi phân tích, nhưng không được sử dụng trong khối ifdef phân tích
	// để dữ liệu có sẵn cho mọi thứ (ví dụ: có thể được sử dụng cho chỉ số trình phát được lưu cục bộ, v.v.)
	public struct DeathEvent
    {
        public string character;
        public string obstacleType;
        public string themeUsed;
        public int coins;
        public int premium;
        public int score;
        public float worldDistance;
    }

    public CharacterInputController controller;

	public ParticleSystem koParticle;

	[Header("Sound")]
	public AudioClip coinSound;
	public AudioClip premiumSound;

    public DeathEvent deathData { get { return m_DeathData; } }
    public new BoxCollider collider { get { return m_Collider; } }

	public new AudioSource audio { get { return m_Audio; } }

    [HideInInspector]
	public List<GameObject> magnetCoins = new List<GameObject>();

    public bool tutorialHitObstacle {  get { return m_TutorialHitObstacle;} set { m_TutorialHitObstacle = value;} }

    protected bool m_TutorialHitObstacle;

    protected bool m_Invincible;
    protected DeathEvent m_DeathData;
	protected BoxCollider m_Collider;
	protected AudioSource m_Audio;

	protected float m_StartingColliderHeight;

    protected readonly Vector3 k_SlidingColliderScale = new Vector3 (1.0f, 0.5f, 1.0f);
    protected readonly Vector3 k_NotSlidingColliderScale = new Vector3(1.0f, 2.0f, 1.0f);

    protected const float k_MagnetSpeed = 10f;
    
    //các lớp layer
    protected const int k_CoinsLayerIndex = 8;
    protected const int k_ObstacleLayerIndex = 9;
    protected const int k_PowerupLayerIndex = 10;

    //Thời gian nhấp nháy
    protected const float k_DefaultInvinsibleTime = 2f;

    protected void Start()
    {
		m_Collider = GetComponent<BoxCollider>();
		m_Audio = GetComponent<AudioSource>();
		m_StartingColliderHeight = m_Collider.bounds.size.y;
	}

	public void Init()
	{
		koParticle.gameObject.SetActive(false);

		s_BlinkingValueHash = Shader.PropertyToID("_BlinkingValue");
		m_Invincible = false;
	}

	public void Slide(bool sliding)
	{
		if (sliding)
		{
			m_Collider.size = Vector3.Scale(m_Collider.size, k_SlidingColliderScale);
			m_Collider.center = m_Collider.center - new Vector3(0.0f, m_Collider.size.y * 0.5f, 0.0f);
		}
		else
		{
			m_Collider.center = m_Collider.center + new Vector3(0.0f, m_Collider.size.y * 0.5f, 0.0f);
			m_Collider.size = Vector3.Scale(m_Collider.size, k_NotSlidingColliderScale);
		}
	}

    protected void Update()
	{
		// Every coin registered to the magnetCoin list (used by the magnet powerup exclusively, but could be used by other power up) is dragged toward the player.
		// Mọi đồng xu được đăng ký trong danh sách magnetCoin (được sử dụng độc quyền bởi bộ tăng sức mạnh nam châm, nhưng có thể được sử dụng bằng sức mạnh khác) được kéo về phía người chơi.
		for (int i = 0; i < magnetCoins.Count; ++i)
		{
            magnetCoins[i].transform.position = Vector3.MoveTowards(magnetCoins[i].transform.position, transform.position, k_MagnetSpeed * Time.deltaTime);
		}
	}

    protected void OnTriggerEnter(Collider c)
    {
		//k_CoinsLayerIndex = số thứ tự của lớp layer
		if (c.gameObject.layer == k_CoinsLayerIndex)
		{
			//Debug.Log(c.gameObject);
			//Contains: xác định mảng magnetCoins có chứa c.gameObject hay không
			if (magnetCoins.Contains(c.gameObject))
				magnetCoins.Remove(c.gameObject);


			//Debug.Log(c.GetComponent<Coin>().isPremium);
			if (c.GetComponent<Coin>().isPremium)
            {
				Addressables.ReleaseInstance(c.gameObject); //giải phóng đối tượng
                PlayerData.instance.premium += 1;
                controller.premium += 1;
				m_Audio.PlayOneShot(premiumSound);
			}
            else
            {
				//sử dụng function Pooler 
				Coin.coinPool.Free(c.gameObject);
                PlayerData.instance.coins += 1;
				controller.coins += 1;
				m_Audio.PlayOneShot(coinSound);
            }
        }
        else if(c.gameObject.layer == k_ObstacleLayerIndex)
        {
            if (m_Invincible || controller.IsCheatInvincible())
                return;

            controller.StopMoving();

			c.enabled = false;

            Obstacle ob = c.gameObject.GetComponent<Obstacle>();

			if (ob != null)
			{
				ob.Impacted();
			}
			else
			{
			    Addressables.ReleaseInstance(c.gameObject);
			}

            if (TrackManager.instance.isTutorial)
            {
                m_TutorialHitObstacle = true;
            }
            else
            {
                controller.currentLife -= 1;
            }

            controller.character.animator.SetTrigger(s_HitHash);

            if (controller.currentLife > 0)
            {
                m_Audio.PlayOneShot(controller.character.hitSound);
                SetInvincible();
            }
            // The collision killed the player, record all data to analytics.
            // Vụ va chạm đã giết chết người chơi, hãy ghi lại tất cả dữ liệu để phân tích.
            else
            {
                m_Audio.PlayOneShot(controller.character.deathSound);

                m_DeathData.character = controller.character.characterName;
                m_DeathData.themeUsed = controller.trackManager.currentTheme.themeName;
                m_DeathData.obstacleType = ob.GetType().ToString();
                m_DeathData.coins = controller.coins;
                m_DeathData.premium = controller.premium;
                m_DeathData.score = controller.trackManager.score;
                m_DeathData.worldDistance = controller.trackManager.worldDistance;
            }
        }
        else if(c.gameObject.layer == k_PowerupLayerIndex)
        {
            //Intance một consuable từ Consuable bằng c.GetComponent<Consumable>();
            Consumable consumable = c.GetComponent<Consumable>();
            //Nếu mà có consumable => thì UseConsumable(consumable)
            if (consumable != null)
            {
                controller.UseConsumable(consumable);
            }
        }
    }

    //va chạm vật thể thì nhấp nháy
    public void SetInvincibleExplicit(bool invincible)
    {
        m_Invincible = invincible;
    }

    public void SetInvincible(float timer = k_DefaultInvinsibleTime)
	{
		StartCoroutine(InvincibleTimer(timer));
	}

    protected IEnumerator InvincibleTimer(float timer)
    {
        m_Invincible = true;

		float time = 0;
		float currentBlink = 1.0f;
		float lastBlink = 0.0f; 
		const float blinkPeriod = 0.1f;

        //nếu time < timer ( timer = k_DefaultInvinsibleTime) 
        while (time < timer && m_Invincible)
		{
            //
			Shader.SetGlobalFloat(s_BlinkingValueHash, currentBlink);

            // We do the check every frame instead of waiting for a full blink period as if the game slow down too much
            // we are sure to at least blink every frame.
            // If blink turns on and off in the span of one frame, we "miss" the blink, resulting in appearing not to blink.
            // Chúng tôi kiểm tra mọi khung hình thay vì đợi một khoảng thời gian nhấp nháy đầy đủ như thể trò chơi chậm lại quá nhiều
            // chúng tôi chắc chắn ít nhất sẽ nhấp nháy mọi khung hình.
            // Nếu nhấp nháy được bật và tắt trong khoảng thời gian của một khung hình, chúng ta "bỏ lỡ" nhấp nháy, dẫn đến xuất hiện không nhấp nháy.
            yield return null;
			time += Time.deltaTime;
			lastBlink += Time.deltaTime;

			if (blinkPeriod < lastBlink)
			{
				lastBlink = 0;
				currentBlink = 1.0f - currentBlink;
			}
        }

		Shader.SetGlobalFloat(s_BlinkingValueHash, 0.0f);

		m_Invincible = false;
    }
}
