using System;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;


[RequireComponent(typeof(Button))]
public class ItemUI : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Button button;


    public void Initialize(string itemType, Item item, Action<string> removeItemAction)
    {
        image.sprite = item.icon;
        transform.localScale = Vector3.one;
        button.onClick.AddListener(() => removeItemAction.Invoke(itemType));
    }

    private void OnDestroy()
    {
        button.onClick.RemoveAllListeners();
    }
}
