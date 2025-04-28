using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DroppedItem : MonoBehaviour
{
   [SerializeField] private bool autoStart;
   [SerializeField] private float enabledPickupDelay = 3.0f;

   public Item item;
   public bool pickedUp = false;

   void Start()
   {
      var collider = GetComponent<Collider>();
      if (collider != null)
      {
         collider.enabled = false; // disable immediately
      }
      
      if (autoStart && item != null)
      {
         Initialize(item);
      }
   }

   public void Initialize(Item item)
   {
      this.item = item;
      var droppedItem = Instantiate(item.prefab, transform);
      droppedItem.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
      StartCoroutine(EnablePickup(enabledPickupDelay));
   }

   IEnumerator EnablePickup(float delay)
   {
      yield return new WaitForSeconds(delay);
      GetComponent<Collider>().enabled = true;
      
   }
}
