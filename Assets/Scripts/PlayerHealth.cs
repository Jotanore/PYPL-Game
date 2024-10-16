/*using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3; // Máximo de puntos de vida
    private int currentHealth; // Puntos de vida actuales

    public Sprite[] healthSprites; // Array de sprites para la barra de vida
    public SpriteRenderer healthBarSpriteRenderer; // Referencia al SpriteRenderer para la barra de vida
    public SpriteRenderer playerSpriteRenderer; // Referencia al SpriteRenderer del jugador
    public GameObject[] reloadIcons; // Array de GameObjects que contienen los iconos de recarga

    public float fadeDuration = 1f; // Duración del desvanecimiento en segundos
    private bool isFading = false; // Estado de desvanecimiento
    private float fadeStartTime;
}// Tiempo en el que comenzó el desvanecimiento

    
        // Inicializar la salud del jugador
        /*
        currentHealth = maxHealth;
        UpdateHealthBar();
        playerSpriteRenderer.color = Color.white; // Asegurarse de que el jugador comienza con opacidad completa
        SetReloadIconsActive(false); // Asegurarse de que los iconos de recarga están activos al inicio
    }
}

    // Método para actualizar la barra de vida
    //
    //
    /*
    void UpdateHealthBar()
    {
        // Asegurarse de que el índice esté dentro del rango válido
        int spriteIndex = Mathf.Clamp(maxHealth - currentHealth, 0, healthSprites.Length - 1);
        if (healthBarSpriteRenderer != null)
        {
            healthBarSpriteRenderer.sprite = healthSprites[spriteIndex];
        }
    }
    */
  
      /*

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
      */