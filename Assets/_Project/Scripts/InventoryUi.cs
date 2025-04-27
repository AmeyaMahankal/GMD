using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class InventoryUi: MonoBehaviour
{
    [SerializeField] GameObject uiItemPrefab;
    [SerializeField] InventoryManager inventory;
    [SerializeField] Transform uiInventoryParent;
    
    SerializedDictionary<string, GameObject> inventoryUI = new();

    public void AddUiItem(string itemType, Item item) {
        var itemUI = Instantiate(uiItemPrefab).GetComponent<ItemUI>();
        itemUI.transform.SetParent(uiInventoryParent);
        inventoryUI.Add(itemType, itemUI.gameObject);
        itemUI.Initialize(itemType, item, inventory.DropItem);
    }

    public void RemoveUiItem(string itemType) {
        var itemUi = inventoryUI.GetValueOrDefault(itemType);
        inventoryUI.Remove(itemType);
        Destroy(itemUi);
    }
}
