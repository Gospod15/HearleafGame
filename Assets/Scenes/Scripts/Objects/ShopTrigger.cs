using UnityEngine;
using UnityEngine.InputSystem;

public class ShopTrigger : MonoBehaviour
{
    public float interactionRadius = 3f;
    private Transform playerTransform;
    private bool isShopOpen = false;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    void Update()
    {
        if (playerTransform == null) return;
        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance <= interactionRadius && Keyboard.current.eKey.wasPressedThisFrame)
        {
            ToggleShop();
        }

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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}