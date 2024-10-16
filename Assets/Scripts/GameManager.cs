using UnityEngine;
using TMPro; // Asegurarse de tener la referencia a TextMeshPro
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public PlayerMovement player;
    public EnemyMovement enemy;
    public TMP_Text turnText; // Referencia al TMP_Text para mostrar el turno actual

    private float turnTimer = 5f; // Temporizador para el turno
    public bool isTurnActive = false; // Estado del turno
    private int turnCount = 0; // Contador de turnos
    public string playerDefeatedScene = "PlayerDefeated";
    public string enemyDefeatedScene = "EnemyDefeated";
    public bool pvp = false;

    void Start()
    {
        // Asignar las referencias entre jugador y enemigo
        if (player != null)
        {
            player.enemy = enemy.gameObject;
        }

        if (enemy != null)
        {
            enemy.player = player.gameObject;
        }

        // Iniciar el primer turno
        StartTurn();
    }

    void Update()
    {
        DeathMenuEnemy();
        DeathMenuPlayer();
        if (isTurnActive)
        {
            turnTimer -= Time.deltaTime;
            if (turnTimer <= 0)
            {
                EndTurn();
            }
        }
    }

    void StartTurn()
    {
        // Iniciar el turno de preparación para el jugador y el enemigo
        if (player != null)
        {
            player.StartPreparationTurn();
        }

        if (enemy != null)
        {
            enemy.StartPreparationTurn();
        }

        // Activar el turno
        isTurnActive = true;
        turnTimer = 5f; // Reiniciar el temporizador
        turnCount++; // Incrementar el contador de turnos
        UpdateTurnText(); // Actualizar el texto del turno
    }

    void EndTurn()
    {
        if (player != null)
        {
            player.ExecuteSelectedAction();
        }

        if (enemy != null)
        {
            enemy.ExecuteSelectedAction();
        }

        // Iniciar el próximo turno
        StartTurn();
    }

    void UpdateTurnText()
    {
        if (turnText != null)
        {
            turnText.text = $"Turn: {turnCount}";
        }
    }


    public void DeathMenuPlayer()
    {
        if (player.playerDead)
        { // Iniciar la corutina de espera y cambio de escena
            StartCoroutine(WaitAndLoadPlayerDefeatedScene());
        } 
        
    }

    private IEnumerator WaitAndLoadPlayerDefeatedScene()
    {
        // Esperar 3 segundos
        yield return new WaitForSeconds(2f);

        // Cargar la nueva escena
        SceneManager.LoadScene(playerDefeatedScene);
    }

    public void DeathMenuEnemy()
    {
        if (enemy.enemyDead)
        {
            // Iniciar la corutina de espera y cambio de escena
            StartCoroutine(WaitAndLoadEnemyDefeatedScene());
            isTurnActive = false;
        }
    }

    private IEnumerator WaitAndLoadEnemyDefeatedScene()
    {
        // Esperar 3 segundos
        yield return new WaitForSeconds(2f);

        // Cargar la nueva escena
        SceneManager.LoadScene(enemyDefeatedScene);
    }
}
