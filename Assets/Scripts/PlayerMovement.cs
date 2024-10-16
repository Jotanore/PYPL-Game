using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public Transform[] tilePositions;
    private int currentPositionIndex = 0;

    public Button leftButton;
    public Button rightButton;
    public Button attackButton;
    public Button guardButton;
    public Button reloadButton;
    public Sprite idleSprite;
    public Sprite reloadSprite;
    public Sprite guardSprite;
    public Sprite[] healthSprites; // Sprites para representar el estado de salud
    public SpriteRenderer spriteRenderer;
    public SpriteRenderer healthBarSpriteRenderer; // SpriteRenderer para la barra de salud
    private Animator animator;
    public GameObject[] reloadIcons; // Array de GameObjects que contienen los iconos de recarga)
    public GameObject reloadIcon1;
    public GameObject reloadIcon2;
    public GameObject reloadIcon3;
    private int reloadCount = 0;

    public string selectedAction = null;
    private float turnTimer = 5f;
    private bool isPreparing = false;
    public bool playerDead = false;

    public Sprite[] timerSprites;
    public SpriteRenderer timerSpriteRenderer;

    public GameObject enemy; // Referencia al objeto EnemyMovement

    private int health = 3; // Cantidad de golpes que puede recibir

    public float fadeDuration = 1f; // Duración del desvanecimiento en segundos
    private bool isFading = false; // Estado de desvanecimiento
    private float fadeStartTime; // Tiempo en el que comenzó el desvanecimiento
    
    //Para cambiar la escena al morir
    public string playerDefeatedScene = "PlayerDefeated";
    //Guarding and damage logic
    public bool playerIsGuarding = false;
    private EnemyMovement enemyMovement;

    void Start()
    {
        /*
        leftButton.onClick.AddListener(() => UpdateSelectedAction("MoveLeft"));
        rightButton.onClick.AddListener(() => UpdateSelectedAction("MoveRight"));
        reloadButton.onClick.AddListener(() => UpdateSelectedAction("Reload"));
        guardButton.onClick.AddListener(() => UpdateSelectedAction("Guard"));
        attackButton.onClick.AddListener(() => UpdateSelectedAction("Attack"));
        */
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        animator.enabled = false;
        spriteRenderer.sprite = idleSprite; // Inicializar con el sprite idle
        enemyMovement = enemy.GetComponent<EnemyMovement>();
        playerDead = false;

        InitializeReloadIcons();
        StartPreparationTurn();
        UpdateHealthSprite(); // Asegura que la barra de salud esté correcta al inicio
    }

    void Update()
    {
        NextAction();
        enemyMovement = enemy.GetComponent<EnemyMovement>();
        if (isPreparing)
        {
            turnTimer -= Time.deltaTime;
            UpdateTimerSprite();
            if (turnTimer <= 0)
            {
                ExecuteSelectedAction();
            }
        }

        if (isFading)
        {
            FadeOut();
        }
    }

    void NextAction()
    {
        if (Input.GetKey(KeyCode.A)) UpdateSelectedAction("MoveLeft");
        if (Input.GetKey(KeyCode.D)) UpdateSelectedAction("MoveRight");
        if (Input.GetKey(KeyCode.S)) UpdateSelectedAction("Reload");
        if (Input.GetKey(KeyCode.E)) UpdateSelectedAction("Guard");
        if (Input.GetKey(KeyCode.W)) UpdateSelectedAction("Attack");
        if (Input.GetKey(KeyCode.Q)) enemyMovement.enemyDead = true;
    }

    void UpdateSelectedAction(string action)
    {
        if (!isPreparing) return;

        selectedAction = action;
        Debug.Log($"Player selected action: {selectedAction}");
    }

    public void ExecuteSelectedAction()
    {
        isPreparing = false;

        if (selectedAction == "MoveRight")
        {
            MoveRight();
            playerIsGuarding = false;
        }
        else if (selectedAction == "MoveLeft")
        {
            MoveLeft();
            playerIsGuarding = false;
        }
        else if (selectedAction == "Reload")
        {
            ReloadAction();
            playerIsGuarding = false;
        }
        else if (selectedAction == "Guard")
        {
            GuardAction();
        }
        else if (selectedAction == "Attack")
        {
            AttackAction();
            playerIsGuarding = false;
        }

        StartPreparationTurn();
    }

    public void StartPreparationTurn()
    {
        selectedAction = null;
        //playerIsGuarding = false;
        turnTimer = 5f;
        isPreparing = true;
        UpdateTimerSprite();
    }

    void InitializeReloadIcons()
    {
        if (reloadIcon1 != null) reloadIcon1.SetActive(false);
        if (reloadIcon2 != null) reloadIcon2.SetActive(false);
        if (reloadIcon3 != null) reloadIcon3.SetActive(false);
    }

    void MoveRight()
    {
        if (currentPositionIndex < tilePositions.Length - 1)
        {
            int newPositionIndex = currentPositionIndex + 1;

            if (enemy != null)
            {
                EnemyMovement enemyMovement = enemy.GetComponent<EnemyMovement>();
                if (enemyMovement != null && enemyMovement.GetCurrentPositionIndex() == newPositionIndex)
                {
                    Debug.Log("Player can't go through Enemy!");
                    return; // Evitar que el jugador se mueva si el enemigo está en la posición deseada
                }
            }

            currentPositionIndex = newPositionIndex;
            UpdatePlayerPosition();
            SetIdleState();
        }
    }

    void MoveLeft()
    {
        if (currentPositionIndex > 0)
        {
            int newPositionIndex = currentPositionIndex - 1;

            if (enemy != null)
            {
                EnemyMovement enemyMovement = enemy.GetComponent<EnemyMovement>();
                if (enemyMovement != null && enemyMovement.GetCurrentPositionIndex() == newPositionIndex)
                {
                    Debug.Log("Player can't go through Enemy!");
                    return; // Evitar que el jugador se mueva si el enemigo está en la posición deseada
                }
            }

            currentPositionIndex = newPositionIndex;
            UpdatePlayerPosition();
            SetIdleState();
        }
    }

    void ReloadAction()
    {
        if (reloadCount < 3)
        {
            reloadCount++;
            UpdateReloadIcons();
        }
        SetStaticSprite(reloadSprite);
    }

    void GuardAction()
    {
        playerIsGuarding = true;

        SetStaticSprite(guardSprite);
    }

    void AttackAction()
    {
        Debug.Log("AttackAction called");

        if (reloadCount > 0)
        {
            Debug.Log("Attack executed");
            TriggerAttackAnimation();
            reloadCount--;
            UpdateReloadIcons();

            if (enemy != null && enemyMovement !=null)
            {
                EnemyMovement enemyMovement = enemy.GetComponent<EnemyMovement>();
                if (enemyMovement.GetCurrentPositionIndex() == currentPositionIndex + 1 || 
                    enemyMovement.GetCurrentPositionIndex() == currentPositionIndex - 1)
                {
                    if (!enemyMovement.enemyIsGuarding)
                    {
                        enemyMovement.TakeDamage(1); // Infligir daño al enemigo
                    }
                }
            }
        }
        else
        {
            Debug.Log("No reloads available");
        }
    }

    void UpdatePlayerPosition()
    {
        transform.position = tilePositions[currentPositionIndex].position;
    }

    void TriggerAttackAnimation()
    {
        animator.enabled = true;
        animator.SetTrigger("AttackTrigger");
    }

    void SetStaticSprite(Sprite sprite)
    {
        animator.enabled = false;
        spriteRenderer.sprite = sprite;
    }

    void SetIdleState()
    {
        animator.enabled = false;
        spriteRenderer.sprite = idleSprite;
    }

    void UpdateReloadIcons()
    {
        if (reloadIcon1 != null) reloadIcon1.SetActive(reloadCount > 0);
        if (reloadIcon2 != null) reloadIcon2.SetActive(reloadCount > 1);
        if (reloadIcon3 != null) reloadIcon3.SetActive(reloadCount > 2);
    }

    void UpdateTimerSprite()
    {
        int spriteIndex = Mathf.Clamp(Mathf.FloorToInt((5f - turnTimer) / 5f * timerSprites.Length), 0, timerSprites.Length - 1);
        timerSpriteRenderer.sprite = timerSprites[spriteIndex];
    }

    public int GetCurrentPositionIndex()
    {
        return currentPositionIndex;
    }

    public void TakeDamage(int damage) 
    {
        
        if (playerIsGuarding) //antes era sin el "= true"
        {
            Debug.Log("Player blocked the attack");
            return; //No recibir daño si esta usando Guard
        }
        
        else //con "else" funciona
        {
        health -= damage;
        Debug.Log("Player took damage. Health: " + health);
            if (health <= 0)
            {
                Debug.Log("Player is defeated!");
                // Aquí puedes agregar la lógica para cuando el jugador es derrotado
            }
            UpdateHealthSprite();
        }
    }
    private void UpdateHealthSprite()
    {
        if (healthBarSpriteRenderer != null)
        {
            if (health == 3)
                healthBarSpriteRenderer.sprite = healthSprites[0];
            
            if (health == 2)
            {
                healthBarSpriteRenderer.sprite = healthSprites[1];
            }
            else if (health == 1)
            {
                healthBarSpriteRenderer.sprite = healthSprites[2];
            }
            else if (health == 0)
            {
                healthBarSpriteRenderer.sprite = healthSprites[3];
                // Aquí puedes añadir la lógica para la muerte del jugador
                Debug.Log("Player is dead!");
                // Iniciar el desvanecimiento y ocultar elementos
                StartFading();
                playerDead = true;
            }
        }
    }

        // Método para iniciar el desvanecimiento
    void StartFading()
    {
        if (!isFading)
        {
            isFading = true;
            fadeStartTime = Time.time; // Registrar el tiempo en que comienza el desvanecimiento
        }
    }

    // Método para desvanecer el SpriteRenderer del jugador
    void FadeOut()
    {
        float elapsed = Time.time - fadeStartTime; // Tiempo transcurrido desde el inicio del desvanecimiento
        float fadeAmount = Mathf.Clamp01(elapsed / fadeDuration); // Calcular la cantidad de desvanecimiento

        // Ajustar la opacidad del jugador
        if (spriteRenderer != null)
        {
            Color playerColor = spriteRenderer.color;
            playerColor.a = 1f - fadeAmount; // Cambiar la opacidad de 100% a 0%
            spriteRenderer.color = playerColor;
        }

        // Ajustar la opacidad de la barra de vida
        if (healthBarSpriteRenderer != null)
        {
            Color healthBarColor = healthBarSpriteRenderer.color;
            healthBarColor.a = 1f - fadeAmount; // Cambiar la opacidad de 100% a 0%
            healthBarSpriteRenderer.color = healthBarColor;
        }

        // Ajustar la opacidad de los iconos de recarga
        if (reloadIcons != null)
        {
            foreach (GameObject icon in reloadIcons)
            {
                if (icon != null)
                {
                    SpriteRenderer iconRenderer = icon.GetComponent<SpriteRenderer>();
                    if (iconRenderer != null)
                    {
                        Color iconColor = iconRenderer.color;
                        iconColor.a = 1f - fadeAmount; // Cambiar la opacidad de 100% a 0%
                        iconRenderer.color = iconColor;
                    }
                }
            }
        }

        // Detener el desvanecimiento y desactivar los elementos si la opacidad es 0
        if (fadeAmount >= 1f)
        {
            isFading = false;
            gameObject.SetActive(false); // Desactivar el jugador

            // También desactivar los iconos de recarga
            if (reloadIcons != null)
            {
                foreach (GameObject icon in reloadIcons)
                {
                    if (icon != null)
                    {
                        icon.SetActive(false);
                    }
                }
            }
        }
    }

    
}

