using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ItemOptionPanel : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private CraftingButton[] craftingItemButtons;
    [SerializeField] private UseItemButton useItemButton;
    [SerializeField] private TextMeshProUGUI removeItemDescription;

    // 아이템 제작 팝업 초기화
    public void InitializeCraftingPopup(int index)
    {
        var itemData = ItemManager.Instance.GetInventoryItem(index);
        itemIcon.sprite = itemData.ItemIcon;
        itemName.text = itemData.ItemName;

        removeItemDescription.text = $"Remove {itemData.ItemName}";

        SetupButtons(itemData);
    }

    public void SetupButtons(ItemBaseSO itemData)
    {
        Debug.Log("SetupButtons 호출");
        Debug.Log($"itemData.ItemName : {itemData.ItemName}");
        Debug.Log($"itemData.ItemType : {itemData.ItemType}");
        Debug.Log($"itemData is UsableItem : {itemData is UsableItem}");

        var availableRecipes = ItemManager.Instance.GetAvailableCraftingRecipes(itemData.ItemType);

        for (int i = 0; i < craftingItemButtons.Length; i++)
            craftingItemButtons[i].gameObject.SetActive(false);

        useItemButton.gameObject.SetActive(false);

        if (itemData is UsableItem) // 아이템이 사용 아이템이라면 사용 버튼 활성화 후 return;
        {
            Debug.Log("아이템이 사용 아이템이라면 사용 버튼 활성화 후 return;");
            useItemButton.gameObject.SetActive(true);
            useItemButton.InitUseItemButton(itemData as UsableItem);
            return;
        }

        if (availableRecipes.Count <= 0)    // 조합 가능한 아이템이 없다면 return;
            return;

        for (int i = 0; i < availableRecipes.Count; i++)    // 조합이 가능하다면 버튼 활성화 후 조합 가능 아이템의 정보를 추가
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
        itemIcon.sprite = null;
        itemName.text = string.Empty;
        removeItemDescription.text = string.Empty;
    }
}
