using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    // 종이 관련 아이템
    Paper,
    Pencil,
    Brush,
    Note,
    Amulet,
    // 기계 관련 아이템
    BrokenRadio,
    RepairedRadio,
    RepairKit,
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
        ItemBaseSO clone = CreateInstance<ItemBaseSO>();

        clone.ItemType = this.ItemType;
        clone.ItemName = this.ItemName;
        clone.ItemDescription = this.ItemDescription;
        clone.ItemIcon = this.ItemIcon;

        return clone;
    }
}