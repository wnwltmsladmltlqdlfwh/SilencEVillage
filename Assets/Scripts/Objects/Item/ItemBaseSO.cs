using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    // 재료 아이템 (드랍 아이템)
    Paper,
    Pencil,
    Brush,
    BrokenRadio,
    RepairKit,
    // 사용(제작) 아이템
    Note,
    Amulet,
    RepairedRadio,
    // 최대 아이템 개수
    ItemMaxCount,
}

[CreateAssetMenu(fileName = "ItemBase", menuName = "Item/ItemBase", order = 0)]
public class ItemBaseSO : ScriptableObject
{
    // 아이템 정보
    public ItemType ItemType;
    public string ItemName;
    public string ItemDescription;
    public Sprite ItemIcon;

    public ItemBaseSO Clone()
    {
        ItemBaseSO clone;

        // 아이템 타입에 따라 아이템 클론 생성
        switch (this.ItemType)
        {
            case ItemType.Note:
                clone = CreateInstance<NoteItem>();
                break;
            case ItemType.Amulet:
                clone = CreateInstance<AmuletItem>();
                break;
            case ItemType.RepairedRadio:
                clone = CreateInstance<RepairRadioItem>();
                break;
            default:
                clone = CreateInstance<ItemBaseSO>();
                break;
        }

        clone.ItemType = this.ItemType;
        clone.ItemName = ItemManager.Instance.ItemNameDictionary[this.ItemType];
        clone.ItemDescription = this.ItemDescription;
        clone.ItemIcon = this.ItemIcon;

        return clone;
    }
}