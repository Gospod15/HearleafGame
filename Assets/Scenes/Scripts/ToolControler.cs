using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;

public class ToolController : MonoBehaviour
{
    [Header("Налаштування Світу")]
    public Tilemap groundTilemap;
    public float toolRange = 3.0f;
    
    [Header("Посилання")]
    public PlayerController player; 
    public GameObject inventoryUI;  

    private ItemData currentTool;

    void Start()
    {
        if (player == null) player = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (inventoryUI != null && inventoryUI.activeSelf) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            UseCurrentTool();
        }
    }

    public void Equip(ItemData tool)
    {
        currentTool = tool;
        Debug.Log($"В руках: {tool.itemName}");
    }

    void UseCurrentTool()
    {
        if (currentTool == null || currentTool.itemType != ItemType.Tool) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);
        mousePos.z = 0;

        if (Vector2.Distance(transform.position, mousePos) > toolRange)
        {
            Debug.Log("Занадто далеко!");
            return;
        }

        if (currentTool.toolType == ToolType.Hoe)
        {
            Vector3Int cellPos = groundTilemap.WorldToCell(mousePos);
            if (groundTilemap.HasTile(cellPos))
            {
                if (currentTool.tileToPlace != null)
                {
                    groundTilemap.SetTile(cellPos, currentTool.tileToPlace);
                    player.ReduceStamina(5f);
                }
            }
        }
        
        else if (currentTool.toolType == ToolType.Pickaxe)
        {
            if (WorldGenerator.instance != null)
            {
                bool mined = WorldGenerator.instance.TryMineStone(mousePos);
                if (mined)
                {
                    player.ReduceStamina(10f);
                    Debug.Log("Камінь видобуто!");
                }
            }
        }

        else if (currentTool.toolType == ToolType.Axe)
        {
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
                TreeScript tree = hit.collider.GetComponent<TreeScript>();
                if (tree != null)
                {
                    tree.TakeDamage(1f);
                    player.ReduceStamina(8f);
                }
            }
        }
    }
}