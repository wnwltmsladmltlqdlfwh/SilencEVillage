using UnityEngine;
using System.Collections.Generic;
using System;

public class ItemManager : Singleton<ItemManager>
{
    // 아이템 정보 관리
    [SerializeField] private static List<ItemBaseSO> itemDataList = new List<ItemBaseSO>();
    public ItemBaseSO GetItemData(ItemType itemType) => itemDataList.Find(item => item.ItemType == itemType);
    public ItemBaseSO GetRandomItemData() => itemDataList[UnityEngine.Random.Range(0, itemDataList.Count)];

    // 아이템 조합 레시피
    [Serializable]
    public struct CraftingRecipe
    {
        public ItemType material1;
        public ItemType material2;
        public ItemType result;

        public CraftingRecipe(ItemType mat1, ItemType mat2, ItemType result)
        {
            this.material1 = mat1;
            this.material2 = mat2;
            this.result = result;
        }
    }

    [SerializeField]
    private List<CraftingRecipe> craftingRecipes = new List<CraftingRecipe>()
    {
        new CraftingRecipe(ItemType.Paper, ItemType.Pencil, ItemType.Note),
        new CraftingRecipe(ItemType.Paper, ItemType.Brush, ItemType.Amulet),
        new CraftingRecipe(ItemType.BrokenRadio, ItemType.RepairKit, ItemType.RepairedRadio),
    };
    private int selectedInventoryIndex = -1;
    public int SelectedInventoryIndex
    {
        get => selectedInventoryIndex;
        set
        {
            if (value < 0 || value >= inventoryItemList.Length)
                return;
            selectedInventoryIndex = value;
        }
    }

    // 현재 인벤토리 아이템 관리
    private ItemBaseSO[] inventoryItemList = new ItemBaseSO[6];
    public bool IsInventoryFull => inventoryItemList.Length >= 6;
    public Action OnInventoryItemChanged;



    // 인벤토리 아이템 가져오기
    public ItemBaseSO GetInventoryItem(int index)
    {
        if (index < 0 || index >= inventoryItemList.Length)
            return null;
        return inventoryItemList[index];
    }

    // 인벤토리 전체 배열 가져오기 (UI 업데이트용)
    public ItemBaseSO[] GetInventoryItems() => inventoryItemList;

    public void AddItem(ItemBaseSO item)
    {
        if (IsInventoryFull)
        {
            Debug.Log("인벤토리가 가득 찼습니다.");
            return;
        }
        for (int i = 0; i < inventoryItemList.Length; i++)
        {
            if (inventoryItemList[i] != null) continue;
            inventoryItemList[i] = item.Clone();
            break;
        }
        OnInventoryItemChanged?.Invoke();
    }

    public void RemoveItem(int index)
    {
        if (index < 0 || index >= inventoryItemList.Length || inventoryItemList[index] == null)
            return;

        inventoryItemList[index] = null;
        OnInventoryItemChanged?.Invoke();
    }

    // 특정 인덱스의 아이템으로 조합 가능한지 확인
    public bool IsCraftable(int itemIndex)
    {
        if (itemIndex < 0 || itemIndex >= inventoryItemList.Length || inventoryItemList[itemIndex] == null)
            return false;

        ItemType itemType = inventoryItemList[itemIndex].ItemType;

        foreach (var recipe in craftingRecipes)
        {
            if (recipe.material1 == itemType || recipe.material2 == itemType)
            {
                // 다른 재료가 인벤토리에 있는지 확인
                ItemType otherMaterial = (recipe.material1 == itemType) ? recipe.material2 : recipe.material1;
                if (HasItemInInventory(otherMaterial))
                {
                    return true;
                }
            }
        }
        return false;
    }

    // 특정 인덱스의 아이템으로 조합 결과 반환
    public ItemType? GetCraftingResult(int itemIndex)
    {
        if (itemIndex < 0 || itemIndex >= inventoryItemList.Length || inventoryItemList[itemIndex] == null)
            return null;

        ItemType itemType = inventoryItemList[itemIndex].ItemType;

        foreach (var recipe in craftingRecipes)
        {
            if (recipe.material1 == itemType || recipe.material2 == itemType)
            {
                ItemType otherMaterial = (recipe.material1 == itemType) ? recipe.material2 : recipe.material1;
                if (HasItemInInventory(otherMaterial))
                {
                    return recipe.result;
                }
            }
        }
        return null;
    }

    // 인벤토리에 특정 아이템이 있는지 확인
    private bool HasItemInInventory(ItemType itemType)
    {
        foreach (var item in inventoryItemList)
        {
            if (item != null && item.ItemType == itemType)
            {
                return true;
            }
        }
        return false;
    }

    // 특정 인덱스의 아이템으로 조합 실행
    public bool TryCraftItem(int itemIndex)
    {
        if (itemIndex < 0 || itemIndex >= inventoryItemList.Length || inventoryItemList[itemIndex] == null)
            return false;

        var result = GetCraftingResult(itemIndex);
        if (result == null) return false;

        ItemType itemType = inventoryItemList[itemIndex].ItemType;

        // 첫 번째 재료 제거 (클릭한 아이템)
        inventoryItemList[itemIndex] = null;

        // 두 번째 재료 찾아서 제거
        var recipe = GetRecipeByMaterial(itemType);
        ItemType otherMaterial = (recipe.Value.material1 == itemType) ? recipe.Value.material2 : recipe.Value.material1;
        RemoveItemFromInventory(otherMaterial);

        // 결과 아이템 추가
        var resultItem = GetItemData(result.Value);
        AddItem(resultItem);

        OnInventoryItemChanged?.Invoke();
        return true;
    }

    // 인벤토리에서 특정 아이템 제거
    private void RemoveItemFromInventory(ItemType itemType)
    {
        for (int i = 0; i < inventoryItemList.Length; i++)
        {
            if (inventoryItemList[i] != null && inventoryItemList[i].ItemType == itemType)
            {
                inventoryItemList[i] = null;
                OnInventoryItemChanged?.Invoke();
                break;
            }
        }
    }

    // 재료로 레시피 찾기
    private CraftingRecipe? GetRecipeByMaterial(ItemType itemType)
    {
        foreach (var recipe in craftingRecipes)
        {
            if (recipe.material1 == itemType || recipe.material2 == itemType)
            {
                ItemType otherMaterial = (recipe.material1 == itemType) ? recipe.material2 : recipe.material1;
                if (HasItemInInventory(otherMaterial))
                {
                    return recipe;
                }
            }
        }
        return null;
    }

    // 특정 아이템으로 조합 가능한 레시피 리스트 반환
    public List<CraftingRecipe> GetAvailableCraftingRecipes(ItemType itemType)
    {
        List<CraftingRecipe> availableRecipes = new List<CraftingRecipe>();
        foreach (var recipe in craftingRecipes)
        {
            if (recipe.material1 == itemType || recipe.material2 == itemType)
            {
                ItemType otherMaterial = (recipe.material1 == itemType) ? recipe.material2 : recipe.material1;
                if (HasItemInInventory(otherMaterial) && !availableRecipes.Contains(recipe))
                {
                    availableRecipes.Add(recipe);
                }
            }
        }
        return availableRecipes;
    }

    // 특정 아이템으로 조합 실행
    public bool TryCraftItemWithRecipe(int itemIndex, CraftingRecipe recipe)
    {
        if (itemIndex < 0 || itemIndex >= inventoryItemList.Length || inventoryItemList[itemIndex] == null)
            return false;

        ItemType itemType = inventoryItemList[itemIndex].ItemType;

        // 레시피가 해당 아이템과 매치되는지 확인
        if (recipe.material1 != itemType && recipe.material2 != itemType)
            return false;

        ItemType otherMaterial = (recipe.material1 == itemType) ? recipe.material2 : recipe.material1;

        // 다른 재료가 인벤토리에 있는지 확인
        if (!HasItemInInventory(otherMaterial))
            return false;

        // 조합 실행
        inventoryItemList[itemIndex] = null;
        RemoveItemFromInventory(otherMaterial);

        var resultItem = GetItemData(recipe.result);

        if(resultItem == null)
            return false;

        AddItem(resultItem);

        OnInventoryItemChanged?.Invoke();
        return true;
    }
}
