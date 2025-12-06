using UnityEngine;
using UnityEngine.InputSystem; // Если используешь Input System

public class ShopTrigger : MonoBehaviour
{
    public float interactionRadius = 3f; // Твой радиус 3
    private Transform playerTransform;
    private bool isShopOpen = false;

    void Start()
    {
        // Ищем игрока по тегу
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        // Считаем дистанцию
        float distance = Vector3.Distance(transform.position, playerTransform.position);

        // Если мы рядом и нажали E
        if (distance <= interactionRadius && Keyboard.current.eKey.wasPressedThisFrame)
        {
            ToggleShop();
        }

        // (Опционально) Закрыть магазин, если отошел далеко
        if (isShopOpen && distance > interactionRadius)
        {
            CloseShopForce();
        }
    }

    void ToggleShop()
    {
        if (ShopManager.instance == null) return;

        isShopOpen = !isShopOpen;

        if (isShopOpen)
            ShopManager.instance.OpenShop();
        else
            ShopManager.instance.CloseShop();
    }

    void CloseShopForce()
    {
        isShopOpen = false;
        ShopManager.instance.CloseShop();
    }

    // Рисует круг в редакторе, чтобы видеть радиус
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}