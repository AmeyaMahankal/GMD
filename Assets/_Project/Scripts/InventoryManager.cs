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
      /*if (other.CompareTag("DroppedItem"))
      {
         var droppedItem = other.GetComponent<DroppedItem>();
         
         if (droppedItem.pickedUp) return;
         var newItem = droppedItem.item;
         var itemType = newItem.itemType;

         if (inventory.ContainsKey(itemType))
         {
            DropItem(itemType);
         }
         
         droppedItem.pickedUp = true;
         AddItem(droppedItem.item);
         Destroy(other.gameObject);
      }*/
   }

   public void AddItem(Item item) {
      inventory.Add(item.itemType, item);
      ui.AddUiItem(item.itemType, item);  
   }

   public void DropItem(string itemType) {
      Vector3 dropOffset = transform.forward + Vector3.up * 1.5f;
      var droppedItem = Instantiate(droppedItemPrefab, transform.position + dropOffset, Quaternion.identity).GetComponent<DroppedItem>();
      var item = inventory.GetValueOrDefault(itemType);
      droppedItem.Initialize(item);
      inventory.Remove(itemType);
      ui.RemoveUiItem(item.itemType);
   }
   
   public void PickupDroppedItem(DroppedItem droppedItem) {
      var newItem = droppedItem.item;
      var newItemType = newItem.itemType;


      if (inventory.ContainsKey(newItemType))
      {
         DropItem(newItemType);
      }
      droppedItem.pickedUp = true;
      AddItem(droppedItem.item);
      Destroy(droppedItem.gameObject);
      
        
   }
}
