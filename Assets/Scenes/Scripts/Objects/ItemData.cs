using UnityEngine;
using UnityEngine.Tilemaps;

public enum ItemType { Resource, Food, Tool }
public enum ToolType { None, Hoe, Axe, Pickaxe }

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public bool isStackable;
    public int maxStackSize = 30;
    public int price;

    [Header("Тип")]
    public ItemType itemType;

    [Header("Параметри Їжі")]
    public int FeedAmount;
    public bool isEatable;

    [Header("Параметри Інструмента")]
    public ToolType toolType; 
    public TileBase tileToPlace;
    public TileBase tileToRemove;
    public GameObject dropPrefab; 
}