using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))] // Гарантирует, что Rigidbody2D есть на объекте
public class PlayerController : MonoBehaviour
{
    [Header("Взаємодія")]
    public float interactionRadius = 2.0f; // Радіус, в якому можна підняти предмет
    public LayerMask interactionLayer;     // Шар предметів (щоб не плутати з ворогами)

    [Header("Настройки Выносливости")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaDrain = 15f; // Расход в секунду
    public float staminaRegen = 7f;  // Восстановление в секунду
    public TextMeshProUGUI staminaText;         // UI текст

    [Header("Настройки Скорости")]
    public float runSpeed = 12f;
    public float walkSpeed = 5f;
    private float currentSpeed;

    [Header("Интерфейс")]
    public Image UIInventory;

    // Скрытые компоненты
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movementInput;
    private string playerName; // Имя с маленькой буквы (camelCase) для приватных полей

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Проверка на GameManager, чтобы избежать ошибок, если запускаем сцену отдельно
        if (GameManager.instance != null)
        {
            playerName = GameManager.instance.playername;
        }

        currentStamina = maxStamina;
        currentSpeed = walkSpeed;

        UpdateStaminaUI();
    }

    void Update()
    {
        // 1. Считывание ввода (Input)
        var keyboard = Keyboard.current;
        if (keyboard == null) return; // Защита, если клавиатура не подключена

        movementInput = Vector2.zero;
        if (keyboard.wKey.isPressed) movementInput.y += 1;
        if (keyboard.sKey.isPressed) movementInput.y -= 1;
        if (keyboard.aKey.isPressed) movementInput.x -= 1;
        if (keyboard.dKey.isPressed) movementInput.x += 1;

        // Нормализуем вектор сразу, чтобы движение по диагонали не было быстрее
        movementInput.Normalize();

        // 2. Обработка действий (Инвентарь и Взаимодействие)
        if (keyboard.iKey.wasPressedThisFrame && UIInventory != null)
        {
            UIInventory.gameObject.SetActive(!UIInventory.gameObject.activeSelf);
        }

        if (keyboard.eKey.wasPressedThisFrame)
        {
            TryInteract();
        }

        // 3. Логика Стамины и Бега
        // Важно: проверяем движение ПОСЛЕ считывания кнопок
        bool isMoving = movementInput.sqrMagnitude > 0; 
        bool isSprinting = keyboard.shiftKey.isPressed;

        HandleStamina(isMoving, isSprinting);

        // 4. Анимация
        UpdateAnimation();
    }

    void FixedUpdate()
    {
        // Физическое передвижение
        rb.MovePosition(rb.position + movementInput * currentSpeed * Time.fixedDeltaTime);
    }

    // --- Вспомогательные методы для чистоты кода ---

    void HandleStamina(bool isMoving, bool isSprinting)
    {
        if (isMoving && isSprinting && currentStamina > 0)
        {
            // Режим бега
            currentSpeed = runSpeed;
            currentStamina -= staminaDrain * Time.deltaTime;
        }
        else
        {
            // Режим ходьбы или стояния
            currentSpeed = walkSpeed;

            // Восстановление энергии
            if (currentStamina < maxStamina)
            {
                currentStamina += staminaRegen * Time.deltaTime;
            }
        }

        // Ограничение значений
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        
        UpdateStaminaUI();
    }

    void UpdateStaminaUI()
    {
        if (staminaText != null)
        {
            // "F0" округляет до целого числа (без запятых)
            staminaText.text = currentStamina.ToString("F0"); 
        }
    }

    void UpdateAnimation()
    {
        if (animator != null)
        {
            animator.SetFloat("X", movementInput.x);
            animator.SetFloat("Y", movementInput.y);
            animator.SetFloat("Speed", movementInput.sqrMagnitude);
        }
    }

    void TryInteract()
    {
        Collider2D hitCollider = Physics2D.OverlapCircle(transform.position, interactionRadius, interactionLayer);

        if (hitCollider != null)
        {
            // Перевіряємо, чи є на об'єкті наш скрипт
            ItemInWorldManager itemInWorld = hitCollider.GetComponent<ItemInWorldManager>();
            
            if (itemInWorld != null)
            {
                itemInWorld.Pickup();
                return; // Якщо підібрали, виходимо (щоб одночасно не копати землю)
            }
        }

        // 2. Якщо предметів немає, пробуємо копати землю (твоя старая логіка)
        bool mined = false;
        if (WorldGenerator.instance != null)
        {
            mined = WorldGenerator.instance.TryMineStone(transform.position);
            if (mined)
            {
                Debug.Log("Rock + 1 (Mining)");
                return;
            }
        }

        Debug.Log("Нічого немає поруч");
    }

    // (Опціонально) Щоб бачити радіус у редакторі Unity
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}