using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 3; // Máximo de puntos de vida
    private int currentHealth; // Puntos de vida actuales

    public Sprite[] healthSprites; // Array de sprites para la barra de vida
    public SpriteRenderer healthBarSpriteRenderer; // Referencia al SpriteRenderer para la barra de vida
    public SpriteRenderer enemySpriteRenderer; // Referencia al SpriteRenderer del enemigo
    public GameObject[] reloadIcons; // Array de GameObjects que contienen los iconos de recarga

    public float fadeDuration = 1f; // Duración del desvanecimiento en segundos
    private bool isFading = false; // Estado de desvanecimiento
    private float fadeStartTime; // Tiempo en el que comenzó el desvanecimiento

    void Start()
    {
        // Inicializar la salud del enemigo
        currentHealth = maxHealth;
        UpdateHealthBar();
        enemySpriteRenderer.color = Color.white; // Asegurarse de que el enemigo comienza con opacidad completa
        SetReloadIconsActive(false); // Asegurarse de que los iconos de recarga están activos al inicio
    }

    // Método para recibir un golpe
    public void TakeDamage()
    {
        if (currentHealth > 0)
        {
            currentHealth--;
            UpdateHealthBar();

            if (currentHealth <= 0)
            {
                // Aquí puedes añadir la lógica para la muerte del enemigo
                Debug.Log("Enemy is dead!");
                // Iniciar el desvanecimiento y ocultar elementos
                StartFading();
            }
        }
    }

    // Método para actualizar la barra de vida
    void UpdateHealthBar()
    {
        // Asegurarse de que el índice esté dentro del rango válido
        int spriteIndex = Mathf.Clamp(maxHealth - currentHealth, 0, healthSprites.Length - 1);
        if (healthBarSpriteRenderer != null)
        {
            healthBarSpriteRenderer.sprite = healthSprites[spriteIndex];
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

    void Update()
    {
        if (isFading)
        {
            FadeOut();
        }
    }

    // Método para desvanecer el SpriteRenderer del enemigo
    void FadeOut()
    {
        float elapsed = Time.time - fadeStartTime; // Tiempo transcurrido desde el inicio del desvanecimiento
        float fadeAmount = Mathf.Clamp01(elapsed / fadeDuration); // Calcular la cantidad de desvanecimiento

        // Ajustar la opacidad del enemigo
        if (enemySpriteRenderer != null)
        {
            Color enemyColor = enemySpriteRenderer.color;
            enemyColor.a = 1f - fadeAmount; // Cambiar la opacidad de 100% a 0%
            enemySpriteRenderer.color = enemyColor;
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
            gameObject.SetActive(false); // Desactivar el enemigo

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

    // Método para activar o desactivar todos los iconos de recarga
    void SetReloadIconsActive(bool isActive)
    {
        if (reloadIcons != null)
        {
            foreach (GameObject icon in reloadIcons)
            {
                if (icon != null)
                {
                    icon.SetActive(isActive);
                }
            }
        }
    }
}
