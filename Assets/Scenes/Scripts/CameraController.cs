using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic; // Потрібно для списків

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 5f;

    [Header("Налаштування Зуму")]
    private float zoomStep = 1f;
    private float minZoom = 3f;
    private float maxZoom = 10f;
    private float zoomDampening = 6f;

    private Camera cam;
    private float targetZoom;
    private float fixedZ = -10f;

    [Header("Налаштування Прозорості (X-Ray)")]
    public LayerMask treeLayer;       // Виберіть шар Tree
    public float transparency = 0.5f; // Наскільки прозорим стає дерево (0.5 = 50%)
    public float checkRadius = 1.5f;  // Радіус навколо гравця для перевірки
    public float yOffsetCheck = 0.5f; // Зміщення перевірки (щоб перевіряти ноги/центр)

    // Список дерев, які зараз прозорі
    private List<SpriteRenderer> obscuredTrees = new List<SpriteRenderer>();

    void Start()
    {
        cam = GetComponent<Camera>();
        targetZoom = cam.orthographicSize;
    }

    private void LateUpdate()
    {
        HandleZoom();
        HandleMovement();
        HandleTransparency(); // Нова логіка
    }

    // --- 1. ЛОГІКА ПРОЗОРОСТІ (НОВЕ) ---
    void HandleTransparency()
    {
        if (target == null) return;

        // 1. Шукаємо всі колайдери дерев у радіусі навколо гравця
        // target.position + Vector3.up * yOffsetCheck — піднімаємо точку перевірки трохи вище ніг
        Collider2D[] hits = Physics2D.OverlapCircleAll(target.position + Vector3.up * yOffsetCheck, checkRadius, treeLayer);

        List<SpriteRenderer> currentHits = new List<SpriteRenderer>();

        foreach (var hit in hits)
        {
            SpriteRenderer sr = hit.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                // ВАЖЛИВА ПЕРЕВІРКА:
                // Ми робимо прозорим дерево, ТІЛЬКИ якщо гравець знаходиться ВИЩЕ (Y) за дерево (тобто за ним),
                // або дуже близько до центру.
                // hit.transform.position.y — це зазвичай точка Pivot (низ стовбура).
                
                if (target.position.y > hit.transform.position.y) 
                {
                    // Робимо напівпрозорим
                    Color color = sr.color;
                    color.a = transparency;
                    sr.color = color;

                    currentHits.Add(sr);
                }
            }
        }

        // 2. Відновлюємо колір дерев, від яких гравець відійшов
        foreach (var oldSr in obscuredTrees)
        {
            // Якщо старого дерева немає в списку нових влучань — відновлюємо його
            if (!currentHits.Contains(oldSr) && oldSr != null)
            {
                Color color = oldSr.color;
                color.a = 1f; // Повна непрозорість
                oldSr.color = color;
            }
        }

        // 3. Оновлюємо список активних дерев
        obscuredTrees = currentHits;
    }

    void HandleZoom()
    {
        float scrollInput = 0f;
        if (Mouse.current != null)
        {
            float rawScroll = Mouse.current.scroll.ReadValue().y;
            if (rawScroll > 0) scrollInput = 1f;
            else if (rawScroll < 0) scrollInput = -1f;
        }

        if (scrollInput != 0f)
        {
            targetZoom -= scrollInput * zoomStep;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        }

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomDampening);
    }

    void HandleMovement()
    {
        if (target == null) return;

        Vector3 desiredPosition = new Vector3(
            target.position.x,
            target.position.y + 0.50f,
            fixedZ
        );

        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, transform.position.y, fixedZ);
    }
}