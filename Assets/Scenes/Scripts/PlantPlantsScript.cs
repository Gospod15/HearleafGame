using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlantPlantsScript : MonoBehaviour
{
    
    public Tilemap tilemap;
    [Header("Налаштування тайлів росту")]

    public TileBase[] LevelOfGrowthWheat;
    public ItemData wheatItemProduct;
    public ItemData wheatItemSeeds;
    public TileBase[] LevelOfGrowthWatermelon;
    [Header("Налаштування тайла грядки")]
    public TileBase TargetDirt;
    public int timeofwheatgrowth = 10;

    void Start()
    {
        
    }
    void Update()
    {
        
    }
    public bool PlantWheat(Vector3 plantPosition)
    {
        Vector3Int cellPos = tilemap.WorldToCell(plantPosition);
        TileBase currentTile = tilemap.GetTile(cellPos);

        if (currentTile == TargetDirt)
        {
            StartCoroutine(GrowWheatRoutine(cellPos));
            return true;
        }

        return false;
    }

    IEnumerator GrowWheatRoutine(Vector3Int pos)
    {
        float waitTime = timeofwheatgrowth / LevelOfGrowthWheat.Length;

        foreach (TileBase stageTile in LevelOfGrowthWheat)
        {
            tilemap.SetTile(pos, stageTile);
            
            yield return new WaitForSeconds(waitTime);
        }
    }

    public bool TryHarvestPlant(Vector3 playerPos)
    {
        Vector3Int cellPos = tilemap.WorldToCell(playerPos);
        TileBase currentTile = tilemap.GetTile(cellPos);

        TileBase matureWheat = LevelOfGrowthWheat[LevelOfGrowthWheat.Length - 1];

        if (currentTile == matureWheat)
        {
            tilemap.SetTile(cellPos, TargetDirt);

            int wheatDropCount = Random.Range(1, 4); 

            for (int i = 0; i < wheatDropCount; i++)
            {
                if (wheatItemProduct != null && wheatItemProduct.dropPrefab != null)
                {
                    Vector3 randomOffset = new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), 0);
                    Instantiate(wheatItemProduct.dropPrefab, playerPos + randomOffset, Quaternion.identity);
                }
            }

            int seedDropCount = Random.Range(1, 3); 

            if (InventoryManager.instance != null && wheatItemSeeds != null)
            {
                InventoryManager.instance.AddItem(wheatItemSeeds, seedDropCount);
            }
            return true;
        }
        return false;
    }
}
