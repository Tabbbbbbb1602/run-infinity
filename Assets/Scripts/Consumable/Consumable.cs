using UnityEngine;
using System.Collections;
using UnityEngine.AddressableAssets;

/// <summary>
/// Defines a consumable (called "power up" in game). Each consumable is derived from this and implements its functions.
/// Định nghĩa một vật phẩm tiêu hao (được gọi là "tăng sức mạnh" trong trò chơi). Mỗi vật tư tiêu hao được bắt nguồn từ điều này và thực hiện các chức năng của nó.
/// </summary>
public abstract class Consumable : MonoBehaviour
{
    //Thời gian diễn ra consumable
    public float duration;

    public enum ConsumableType
    {
        NONE,
        COIN_MAG,
        SCORE_MULTIPLAYER,
        INVINCIBILITY,
        EXTRALIFE,
        SUPERLIFE,
        TAB,
        STAR,
        TABx4,
        MAX_COUNT
    }

    public Sprite icon;
	public AudioClip activatedSound;
    //public ParticleSystem activatedParticle;
    public AssetReference ActivatedParticleReference;
    public bool canBeSpawned = true;

    public bool active {  get { return m_Active; } }
    public float timeActive {  get { return m_SinceStart; } }

    protected bool m_Active = true;
    protected float m_SinceStart;
    protected ParticleSystem m_ParticleSpawned;

    // Here - for the sake of showing diverse way of doing things - we use abstract functions to get the data for each consumable.
    // Another way to do it would be to have public field, like the Character or Accesories use, and define all those on the prefabs instead of here.
    // This method allows information to be all in code (so no need for prefab etc.) the other make it easier to modify without recompiling/by non-programmer.
    public abstract ConsumableType GetConsumableType();
    public abstract string GetConsumableName();
    public abstract int GetPrice();
	public abstract int GetPremiumCost();

    public void ResetTime()
    {
        //reset lại time khi nhặt trùng consumable
        m_SinceStart = 0;
    }

    //override this to do test to make a consumable not usable (e.g. used by the ExtraLife to avoid using it when at full health)
    // Character có thể sử dụng hay không
    public virtual bool CanBeUsed(CharacterInputController c)
    {
        return true;
    }

    public virtual IEnumerator Started(CharacterInputController c)
    {
        m_SinceStart = 0;

		if (activatedSound != null)
		{
			c.powerupSource.clip = activatedSound;
			c.powerupSource.Play();
		}

        if(ActivatedParticleReference != null)
        {
            //Addressables 1.0.1-preview
            // khai báo một object bằng cách tạo
            var op = ActivatedParticleReference.InstantiateAsync();
            //trả về obj
            yield return op;
            //GetComponent<ParticleSystem>() của op
            m_ParticleSpawned = op.Result.GetComponent<ParticleSystem>();
            if (!m_ParticleSpawned.main.loop)
                StartCoroutine(TimedRelease(m_ParticleSpawned.gameObject, m_ParticleSpawned.main.duration));
            //effect theo thằng character
            m_ParticleSpawned.transform.SetParent(c.characterCollider.transform );
            //vị trí mà xuất hiện effect
            m_ParticleSpawned.transform.localPosition = op.Result.transform.position;
        }
	}

    //Thời gian thực hiện effect
    IEnumerator TimedRelease(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);
        Addressables.ReleaseInstance(obj);
    }

    //đánh dấu consunable đang dùng
    public virtual void Tick(CharacterInputController c)
    {
        // By default do nothing, override to do per frame manipulation
        // Theo mặc định, không làm gì cả, ghi đè để thực hiện thao tác trên mỗi khung hình
        m_SinceStart += Time.deltaTime;
        if (m_SinceStart >= duration)
        {
            m_Active = false;
            return;
        }
    }

    public virtual void Ended(CharacterInputController c)
    {
        if (m_ParticleSpawned != null)
        {
            if (m_ParticleSpawned.main.loop)
                // Huỷ đi m_ParticleSpawned.gameObject
                Addressables.ReleaseInstance(m_ParticleSpawned.gameObject);
        }

        if (activatedSound != null && c.powerupSource.clip == activatedSound)
            c.powerupSource.Stop(); //if this one the one using the audio source stop it

        for (int i = 0; i < c.consumables.Count; ++i)
        {
            if (c.consumables[i].active && c.consumables[i].activatedSound != null)
            {//if there is still an active consumable that have a sound, this is the one playing now
                //nếu vẫn còn một vật phẩm đang hoạt động mà có âm thanh, thì đây là vật phẩm đang phát
                c.powerupSource.clip = c.consumables[i].activatedSound;
                c.powerupSource.Play();
            }
        }
    }
}
