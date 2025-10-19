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
        craftingDescription.text = $"{recipe.material1.ToString()} + {recipe.material2.ToString()} = {recipe.result.ToString()}";
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnClickCraftingButton();
    }

    public void OnClickCraftingButton()
    {
        ItemManager.Instance.TryCraftItemWithRecipe(ItemManager.Instance.SelectedInventoryIndex, recipe);
        UIManager.Instance.CloseCraftingPopup();
    }
}