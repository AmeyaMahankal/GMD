using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item", order = 1)]
public class Item : ScriptableObject
{
    public string itemType;
    public string itemName;
    public Sprite icon;
    public GameObject prefab;
    [CanBeNull] public string description;
}

