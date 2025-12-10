using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public string saveName;

    public int worldSeed;

    public float[] playerPosition = new float[3]; // X, Y, Z
    public float currentHealth;
    public float currentFeed;
    public float currentStamina;

    public List<InventoryItemData> inventory = new List<InventoryItemData>();
}

[System.Serializable]
public class InventoryItemData
{
    public string itemName;
    public int amount;
    public bool isShopSlot;
}