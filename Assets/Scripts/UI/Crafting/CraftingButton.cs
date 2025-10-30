using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class CraftingButton : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private ItemManager.CraftingRecipe recipe;
    [SerializeField] private TextMeshProUGUI craftingDescription;

    public void InitializeCraftingButton(ItemManager.CraftingRecipe recipe)
    {
        this.recipe = recipe;
        var nameDict = ItemManager.Instance.ItemNameDictionary;
        craftingDescription.text = $"{nameDict[recipe.material1]}\n+ {nameDict[recipe.material2]}\n= {nameDict[recipe.result]}";
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnClickCraftingButton();
    }

    public void OnClickCraftingButton()
    {
        if(UIManager.Instance.IsProgress) return;

        SoundManager.Instance.PlaySFX(SoundManager.SFXType.Player_MakeItem);
        UIManager.Instance.StartItemCraftingProgress();
        ItemManager.Instance.TryCraftItemWithRecipe(ItemManager.Instance.SelectedInventoryIndex, recipe);
        UIManager.Instance.CloseCraftingPopup();
    }
}