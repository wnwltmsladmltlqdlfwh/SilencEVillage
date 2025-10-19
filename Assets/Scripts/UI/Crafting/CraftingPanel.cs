using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CraftingPanel : MonoBehaviour
{
    [SerializeField] private Image materialItemIcon;
    [SerializeField] private TextMeshProUGUI materialItemName;
    [SerializeField] private CraftingButton[] craftingItemButtons;
    [SerializeField] private Button removeItemButton;
    [SerializeField] private TextMeshProUGUI removeItemDescription;

    // 아이템 제작 팝업 초기화
    public void InitializeCraftingPopup(int index)
    {
        var itemData = ItemManager.Instance.GetInventoryItem(index);
        materialItemIcon.sprite = itemData.ItemIcon;
        materialItemName.text = itemData.ItemName;

        removeItemButton.gameObject.SetActive(true);
        removeItemDescription.text = $"Remove {itemData.ItemName}";

        SetupCraftingButtons(itemData);
    }

    public void SetupCraftingButtons(ItemBaseSO itemData)
    {
        var availableRecipes = ItemManager.Instance.GetAvailableCraftingRecipes(itemData.ItemType);

        for (int i = 0; i < craftingItemButtons.Length; i++)
        {
            craftingItemButtons[i].gameObject.SetActive(false);
        }

        if (availableRecipes.Count <= 0)
            return;

        for (int i = 0; i < availableRecipes.Count; i++)
        {
            craftingItemButtons[i].gameObject.SetActive(true);
            craftingItemButtons[i].InitializeCraftingButton(availableRecipes[i]);
        }
    }

    public void OnClickRemoveItem()
    {
        ItemManager.Instance.RemoveItem(ItemManager.Instance.SelectedInventoryIndex);
        UIManager.Instance.CloseCraftingPopup();
    }

    public void OnClickCloseCraftingPopup()
    {
        ItemManager.Instance.SelectedInventoryIndex = -1;
        UIManager.Instance.CloseCraftingPopup();
    }

    public void ClearCraftingPopup()
    {
        materialItemIcon.sprite = null;
        materialItemName.text = string.Empty;
        removeItemDescription.text = string.Empty;
    }
}
