using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Collider))]
public class InventoryManager : MonoBehaviour
{
   [SerializeField] InventoryUi ui;
   [SerializeField] AudioSource audio;
   
   
   [SerializeField] GameObject droppedItemPrefab;
   
   SerializedDictionary<string, Item > inventory = new SerializedDictionary<string, Item>();

   public void OnTriggerEnter(Collider other)
   {
      if (other.CompareTag("DroppedItem"))
      {
         var droppedItem = other.GetComponent<DroppedItem>();
         
         if (droppedItem.pickedUp) return;
         
         droppedItem.pickedUp = true;
         AddItem(droppedItem.item);
         Destroy(other.gameObject);
      }
   }

   public void AddItem(Item item) {
      inventory.Add(item.itemType, item);
      ui.AddUiItem(item.itemType, item);
   }

   public void DropItem(string itemType) {
      var droppedItem = Instantiate(droppedItemPrefab, transform.position, Quaternion.identity).GetComponent<DroppedItem>();
      var item = inventory.GetValueOrDefault(itemType);
      droppedItem.Initialize(item);
      inventory.Remove(itemType);
      ui.RemoveUiItem(item.itemType);
   }
}
