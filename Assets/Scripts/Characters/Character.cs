using UnityEngine;
using System;

/// <summary>
/// Mainly used as a data container to define a character. This script is attached to the prefab
/// (found in the Bundles/Characters folder) and is to define all data related to the character.
/// Chủ yếu được sử dụng như một vùng chứa dữ liệu để xác định một ký tự. Tập lệnh này được đính kèm với nhà lắp ghép
/// (được tìm thấy trong thư mục Bundles / Characters) và là để xác định tất cả dữ liệu liên quan đến ký tự.
/// </summary>
public class Character : MonoBehaviour
{
    public string characterName;
    public int cost;
	public int premiumCost;

	public CharacterAccessories[] accessories;

    public Animator animator;
	public Sprite icon;

	[Header("Sound")]
	public AudioClip jumpSound;
	public AudioClip hitSound;
	public AudioClip deathSound;

    // Called by the game when an accessory changes, enable/disable the accessories children objects accordingly
    // a value of -1 as parameter disables all accessory.
    //Trò chơi được gọi khi một phụ kiện thay đổi, bật / tắt phụ kiện trẻ em đối tượng tương ứng
    //giá trị -1 khi tham số vô hiệu hóa tất cả phụ kiện.
    public void SetupAccesory(int accessory)
    {
        for (int i = 0; i < accessories.Length; ++i)
        {
            accessories[i].gameObject.SetActive(i == PlayerData.instance.usedAccessory);
        }
    }
}
