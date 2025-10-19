using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private int slotIndex;
    [SerializeField] private ItemBaseSO itemData;
    [SerializeField] private Image itemIcon;

    public void SetItem(ItemBaseSO item, int index)
    {
        slotIndex = index;
        itemData = item;
        itemIcon.sprite = itemData.ItemIcon;
    }

    public void ClearSlot()
    {
        itemData = null;
        itemIcon.sprite = null;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(itemData == null) return;

        ItemManager.Instance.SelectedInventoryIndex = slotIndex;
        UIManager.Instance.ShowCraftingPopup();
    }
}
