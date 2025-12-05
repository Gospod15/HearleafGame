using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public bool isStackable;
    public int maxStackSize = 30;
    
    [TextArea] public string description;

    // НОВОЕ: Какой префаб создавать в мире, если выбросить этот предмет
    public GameObject dropPrefab; 
}