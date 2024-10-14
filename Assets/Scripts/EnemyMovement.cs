using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class EnemyMovement : MonoBehaviour
{
    public Transform[] tilePositions;
    private int currentPositionIndex = 8;

    public Sprite idleSprite;
    public Sprite reloadSprite;
    public Sprite guardSprite;
    public Sprite attackSprite;
    public SpriteRenderer spriteRenderer; // SpriteRenderer principal del enemigo
    public SpriteRenderer healthBarSpriteRenderer; // SpriteRenderer para la barra de salud
    public Sprite[] healthSprites; // Sprites para representar el estado de salud
    private Animator animator;
    public GameObject[] reloadIcons; // Array de GameObjects que contienen los iconos de recarga)
    public GameObject reloadIcon1;
    public GameObject reloadIcon2;
    public GameObject reloadIcon3;

    private string selectedAction = null;
    private bool isPreparing = false;
    private float turnTimer = 5f;
    private int reloadCount = 0;

    public GameObject player;

    private int health = 3; // Cantidad de golpes que puede recibir

    public float fadeDuration = 1f; // Duración del desvanecimiento en segundos
    private bool isFading = false; // Estado de desvanecimiento
    private float fadeStartTime; // Tiempo en el que comenzó el desvanecimiento
    
    //Para cambiar la escena al morir
    public string enemyDefeatedScene = "EnemyDefeated";
    public GameObject gameControllerObject;
    private GameController gameControllerScript;
    
    //Guarding and damage logic
    public bool enemyIsGuarding = false;
    private PlayerMovement playerMovement;

    void Start()
    {
       
        animator = GetComponent<Animator>();
        animator.enabled = false;
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerMovement = player.GetComponent<PlayerMovement>();
        gameControllerScript = gameControllerObject.GetComponent<GameController>(); //para el deathScene
        InitializeReloadIcons();
        StartPreparationTurn();
    }

    void Update()
    {
        playerMovement = player.GetComponent<PlayerMovement>();
        if (isPreparing)
        {
            turnTimer -= Time.deltaTime;
            if (turnTimer <= 0)
            {
                ExecuteSelectedAction();
            }
        }

        if (isFading)
        {
            FadeOut();
            DeathMenuEnemy();
        }
    }

    public void StartPreparationTurn()
    {
        if (!isPreparing)
        {
            selectedAction = GetRandomAction();
            //enemyIsGuarding = false;
            turnTimer = 5f;
            isPreparing = true;
            Debug.Log("Enemy started preparing: " + selectedAction);
        }
    }

    public void ExecuteSelectedAction()
    {
        if (!isPreparing) return;
        isPreparing = false;

        Debug.Log("Enemy executing action: " + selectedAction);

        if (selectedAction == "MoveRight")
        {
            MoveRight();
            enemyIsGuarding = false;
        }
        else if (selectedAction == "MoveLeft")
        {
            MoveLeft();
            enemyIsGuarding = false;
        }
        else if (selectedAction == "Reload")
        {
            ReloadAction();
            enemyIsGuarding = false;
        }
        else if (selectedAction == "Guard")
        {
            GuardAction();
        }
        else if (selectedAction == "Attack")
        {
            AttackAction();
            enemyIsGuarding = false;
        }

        UpdateReloadIcons();
    }

    void MoveRight()
    {
        if (currentPositionIndex < tilePositions.Length - 1)
        {
            int newPositionIndex = currentPositionIndex + 1;

            if (player != null)
            {
                PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
                if (playerMovement != null && playerMovement.GetCurrentPositionIndex() == newPositionIndex)
                {
                    Debug.Log("Enemy can't go through Player!");
                    return;
                }
            }

            currentPositionIndex = newPositionIndex;
            UpdateEnemyPosition();
        }
    }

    void MoveLeft()
    {
        if (currentPositionIndex > 0)
        {
            int newPositionIndex = currentPositionIndex - 1;

            if (player != null)
            {
                PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
                if (playerMovement != null && playerMovement.GetCurrentPositionIndex() == newPositionIndex)
                {
                    Debug.Log("Enemy can't go through Player!");
                    return;
                }
            }

            currentPositionIndex = newPositionIndex;
            UpdateEnemyPosition();
        }
    }

    void ReloadAction()
    {
        if (reloadCount < 3)
        {
            reloadCount++;
        }
        SetStaticSprite(reloadSprite);
    }

    void GuardAction()
    {
        enemyIsGuarding = true;
        SetStaticSprite(guardSprite);
    }

    void AttackAction()
    {
        if (reloadCount > 0)
        {
            Debug.Log("Enemy attacks!");
            TriggerAttackAnimation();
            reloadCount--;

            if (player != null && playerMovement != null)
            {
                PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
                if (playerMovement.GetCurrentPositionIndex() == currentPositionIndex + 1 || 
                    playerMovement.GetCurrentPositionIndex() == currentPositionIndex - 1)
                {   
                    if (!playerMovement.playerIsGuarding)
                    {
                        playerMovement.TakeDamage(1); // Infligir daño al jugador
                    }
                }
            }
        }
        else
        {
            Debug.Log("Enemy has no reloads to attack.");
        }

        }

    void UpdateEnemyPosition()
    {
        transform.position = tilePositions[currentPositionIndex].position;
        SetStaticSprite(idleSprite);
    }

    void TriggerAttackAnimation()
    {
        animator.enabled = true;
        animator.SetTrigger("AttackTrigger");
    }

    void SetStaticSprite(Sprite sprite)
    {
        if (animator != null)
        {
            animator.enabled = false;
        }
        spriteRenderer.sprite = sprite;
    }

    void InitializeReloadIcons()
    {
        if (reloadIcon1 != null) reloadIcon1.SetActive(false);
        if (reloadIcon2 != null) reloadIcon2.SetActive(false);
        if (reloadIcon3 != null) reloadIcon3.SetActive(false);
    }

    void UpdateReloadIcons()
    {
        if (reloadIcon1 != null) reloadIcon1.SetActive(reloadCount > 0);
        if (reloadIcon2 != null) reloadIcon2.SetActive(reloadCount > 1);
        if (reloadIcon3 != null) reloadIcon3.SetActive(reloadCount > 2);
    }

    public int GetCurrentPositionIndex()
    {
        return currentPositionIndex;
    }

    public void TakeDamage(int damage)
    {
        if (enemyIsGuarding) //antes era sin el "= true"
        {
            Debug.Log("Enemy blocked the attack");
            return; //No recibir daño si esta usando Guard
        }
        else //con "else" funciona
        {
        health -= damage;
        Debug.Log("Enemy took damage. Health: " + health);
            if (health <= 0)
            {
             Debug.Log("Enemy is defeated!");
            // Aquí puedes agregar la lógica para cuando el enemigo es derrotado
            }
            UpdateHealthSprite();
        }
    }
    private void UpdateHealthSprite()
    {
        if (healthBarSpriteRenderer != null)
        {
            if (health == 3)
            {
                healthBarSpriteRenderer.sprite = healthSprites[0];
            }
            
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
                // Aquí puedes agregar la lógica para la muerte del enemigo
            // Aquí puedes añadir la lógica para la muerte del jugador
                Debug.Log("Enemy is dead!");
                // Iniciar el desvanecimiento y ocultar elementos
                StartFading();
                DeathMenuEnemy();
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

    private string GetRandomAction()
    {
        string[] actions = new string[] { "MoveRight", "MoveLeft", "Reload", "Guard", "Attack" };
        return actions[Random.Range(0, actions.Length)];
    }

    public void DeathMenuEnemy()
    {
        // Iniciar la corutina de espera y cambio de escena
        StartCoroutine(WaitAndLoadEnemyDefeatedScene());
        GameController gameControllerScript = gameControllerObject.GetComponent<GameController>();
        gameControllerScript.isTurnActive = false;
    }

    private IEnumerator WaitAndLoadEnemyDefeatedScene()
    {
        // Esperar 3 segundos
        yield return new WaitForSecondsRealtime(2f);

        // Cargar la nueva escena
        SceneManager.LoadScene(enemyDefeatedScene);
    }
}