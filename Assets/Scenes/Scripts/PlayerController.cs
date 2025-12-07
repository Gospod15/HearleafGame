using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))] // Гарантирует, что Rigidbody2D есть на объекте
public class PlayerController : MonoBehaviour
{
<<<<<<< Updated upstream
    [Header("Настройки Выносливости")]
=======
    [Header("Здоров'я та Голод")]
    public float maxHealth = 100.0f;
    public float currentHealth;
    
    public float maxFeed = 100.0f;
    public float currentFeed;
    
    // Щоб голод віднімався довше (повільніше), зменшуємо це число.
    // 0.05 = 1 одиниця голоду за 20 секунд.
    // 0.01 = 1 одиниця за 100 секунд.
    public float FeedDrain = 0.05f; 
    public float starvationDamage = 1f;

    [Header("Стаміна")]
>>>>>>> Stashed changes
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaDrain = 15f; // Расход в секунду
    public float staminaRegen = 7f;  // Восстановление в секунду
    public TextMeshProUGUI staminaText;         // UI текст

    [Header("Настройки Скорости")]
    public float runSpeed = 12f;
    public float walkSpeed = 5f;
    private float currentSpeed;

<<<<<<< Updated upstream
    [Header("Интерфейс")]
    public Image UIInventory;

    // Скрытые компоненты
=======
    [Header("Збирання (Руками на E)")]
    public float interactionRadius = 2.0f; 
    public LayerMask interactionLayer; 

    // Приватні компоненти
    private ToolController toolController;
>>>>>>> Stashed changes
    private Rigidbody2D rb;
    private Animator animator;
    
    // Рух
    private Vector2 movementInput;
<<<<<<< Updated upstream
    private string playerName; // Имя с маленькой буквы (camelCase) для приватных полей
=======
    public float runSpeed = 7f;
    public float walkSpeed = 4f;
    private float currentSpeed;
>>>>>>> Stashed changes

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

<<<<<<< Updated upstream
        // Проверка на GameManager, чтобы избежать ошибок, если запускаем сцену отдельно
        if (GameManager.instance != null)
        {
            playerName = GameManager.instance.playername;
        }

        currentStamina = maxStamina;
        currentSpeed = walkSpeed;

        UpdateStaminaUI();
=======
        // Стандартні значення
        currentHealth = maxHealth;
        currentFeed = maxFeed;
        currentStamina = maxStamina;
        currentSpeed = walkSpeed;

        // Завантаження даних з GameManager (якщо є збереження)
        if (GameManager.instance != null && GameManager.instance.currentLoadedData != null)
        {
            SaveData data = GameManager.instance.currentLoadedData;

            Vector3 loadedPos = new Vector3(data.playerPosition[0], data.playerPosition[1], data.playerPosition[2]);
            transform.position = loadedPos;

            currentHealth = data.currentHealth;
            currentFeed = data.currentFeed;
            currentStamina = data.currentStamina;
        }

        // Ховаємо інвентар на старті
        if (UIInventory != null)
        {
            UIInventory.gameObject.SetActive(false);
        }
        
        UpdateUI();
>>>>>>> Stashed changes
    }

    void Update()
    {
<<<<<<< Updated upstream
        // 1. Считывание ввода (Input)
=======
        if (currentHealth <= 0) return; // Якщо мертвий - нічого не робимо

>>>>>>> Stashed changes
        var keyboard = Keyboard.current;
        if (keyboard == null) return; // Защита, если клавиатура не подключена

        // --- ВВІД РУХУ ---
        movementInput = Vector2.zero;
        if (keyboard.wKey.isPressed) movementInput.y += 1;
        if (keyboard.sKey.isPressed) movementInput.y -= 1;
        if (keyboard.aKey.isPressed) movementInput.x -= 1;
        if (keyboard.dKey.isPressed) movementInput.x += 1;

        // Нормализуем вектор сразу, чтобы движение по диагонали не было быстрее
        movementInput.Normalize();

<<<<<<< Updated upstream
        // 2. Обработка действий (Инвентарь и Взаимодействие)
        if (keyboard.iKey.wasPressedThisFrame && UIInventory != null)
        {
            UIInventory.gameObject.SetActive(!UIInventory.gameObject.activeSelf);
=======
        // --- ІНВЕНТАР (ВІДКРИТТЯ/ЗАКРИТТЯ) ---
        if (keyboard.iKey.wasPressedThisFrame && UIInventory != null)
        {
            bool isActive = !UIInventory.gameObject.activeSelf;
            UIInventory.gameObject.SetActive(isActive);
            
            // Оновлюємо картинки в інвентарі, щоб не було багів
            if (isActive && InventoryManager.instance != null)
            {
                InventoryManager.instance.RefreshUI();
            }

            // Скидаємо виділення предмета
            if (ContextMenuController.instance != null)
            {
                ContextMenuController.instance.ClearSelection();
            }
>>>>>>> Stashed changes
        }

        // --- ВЗАЄМОДІЯ (E) ---
        if (keyboard.eKey.wasPressedThisFrame)
        {
            TryInteract();
        }

<<<<<<< Updated upstream
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
=======
        // --- СТАМІНА ТА БІГ ---
        bool isSprinting = keyboard.shiftKey.isPressed;
        HandleMovementAndStamina(isSprinting);
        
        // --- ГОЛОД ---
        HandleHunger();
        
        // --- АНІМАЦІЯ ---
        UpdateAnimation();
    }

    void FixedUpdate() 
    { 
        if (currentHealth > 0) 
        {
            rb.MovePosition(rb.position + movementInput * currentSpeed * Time.fixedDeltaTime); 
        }
    }

    // --- РОЗГОРНУТІ МЕТОДИ ---

    void TryInteract() 
    { 
        // 1. Шукаємо предмети, що валяються на землі (префаби)
        Collider2D hit = Physics2D.OverlapCircle(transform.position, interactionRadius, interactionLayer); 
        
        if (hit != null) 
        { 
            ItemInWorldManager itemInWorld = hit.GetComponent<ItemInWorldManager>(); 
            if (itemInWorld != null) 
            { 
                itemInWorld.Pickup(); 
                return; // Якщо підібрали - виходимо
            } 
        } 
        
        // 2. Якщо предметів немає, пробуємо добути ресурс з тайла (камінь, кущ)
        if (WorldGenerator.instance != null) 
        { 
            WorldGenerator.instance.TryMineStone(transform.position); 
        } 
    }

    public void EquipItem(ItemData item) 
    { 
        if (toolController != null) 
        {
            toolController.Equip(item); 
        }
    }

    public void ReduceStamina(float amount) 
    { 
        currentStamina -= amount; 
        
        if (currentStamina < 0) 
        {
            currentStamina = 0; 
        }
        
        UpdateUI(); 
    }

    public void Eat(float amount) 
    { 
        currentFeed += amount; 
        
        if (currentFeed > maxFeed) 
        {
            currentFeed = maxFeed; 
        }
        
        UpdateUI(); 
    }

    void HandleHunger() 
    { 
        if (currentFeed > 0) 
        {
            // Віднімаємо голод з часом
            currentFeed -= FeedDrain * Time.deltaTime; 
        }
        else 
        { 
            // Якщо голод 0 - наносимо урон
            currentFeed = 0; 
            TakeDamage(starvationDamage * Time.deltaTime); 
        } 
        
        UpdateUI(); 
    }

    public void TakeDamage(float damage) 
    { 
        currentHealth -= damage; 
        
        UpdateUI(); 
        
        if (currentHealth <= 0) 
        {
            Die(); 
        }
    }

    void HandleMovementAndStamina(bool isSprinting) 
    { 
        bool isMoving = movementInput.sqrMagnitude > 0; 
        bool isStarving = currentFeed <= 0; // Якщо голодний - бігати не можна

        if (isMoving && isSprinting && currentStamina > 0 && !isStarving) 
        { 
            currentSpeed = runSpeed; 
            currentStamina -= staminaDrain * Time.deltaTime; 
        } 
        else 
        { 
            currentSpeed = walkSpeed; 
            
            if (currentStamina < maxStamina) 
            {
                currentStamina += staminaRegen * Time.deltaTime; 
            }
        } 
    }

    void Die() 
    { 
        if (animator != null) 
        {
            animator.SetBool("IsDead", true); 
        }
        
        this.enabled = false; // Вимикаємо керування
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

    void UpdateUI() 
    { 
        if (healthText != null) 
        {
            healthText.text = currentHealth.ToString("F0"); 
        }
        
        if (feedText != null) 
        {
            feedText.text = currentFeed.ToString("F0"); 
        }
        
        if (staminaText != null) 
        {
>>>>>>> Stashed changes
            staminaText.text = currentStamina.ToString("F0"); 
        }
    }

<<<<<<< Updated upstream
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
        bool mined = false;

        if (WorldGenerator.instance != null)
        {
            mined = WorldGenerator.instance.TryMineStone(transform.position);
            if (mined)
            {
                Debug.Log("Rock + 1");
                return; // Если выкопали, выходим, чтобы не спамить логами
            }
        }

        Debug.Log("Нечего подбирать / Невдалося забрати камінь");
=======
    void OnDrawGizmosSelected() 
    { 
        Gizmos.color = Color.yellow; 
        Gizmos.DrawWireSphere(transform.position, interactionRadius); 
>>>>>>> Stashed changes
    }
}