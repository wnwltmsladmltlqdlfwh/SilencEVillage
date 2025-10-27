using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IPointerDownHandler
{
    // 지연 초기화를 위한 매니저 참조
    private UIManager _uiManager;
    private ItemManager _itemManager;
    // 프로퍼티로 안전한 접근
    private UIManager uiManager => _uiManager != null ? _uiManager : (_uiManager = UIManager.Instance);
    private ItemManager itemManager => _itemManager != null ? _itemManager : (_itemManager = ItemManager.Instance);

    [SerializeField] private int slotIndex;
    [SerializeField] private ItemBaseSO itemData;
    [SerializeField] private Image itemIcon;

    public void SetItem(ItemBaseSO item, int index)
    {
        slotIndex = index;
        itemData = item;
        itemIcon.sprite = itemData.ItemIcon;
        itemIcon.color = new Color(1f, 1f, 1f, 1f);
    }

    public void ClearSlot()
    {
        itemData = null;
        itemIcon.sprite = null;
        itemIcon.color = new Color(0f, 0f, 0f, 0f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(itemData == null || uiManager.IsMoving || uiManager.IsProgress) return;

        itemManager.SelectedInventoryIndex = slotIndex;
        uiManager.ShowCraftingPopup();
    }
}
