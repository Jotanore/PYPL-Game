using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public static MainMenu Instance;
    public GameManager gameManager;

    public string gameSceneName = "Sample Scene"; // Nombre de la escena del juego
    public bool pvp;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void PlayGame()
    {
        
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnToggleChange(bool tick)
    {
        if (tick)
        {
            pvp = true;
        }
        else
        {
            pvp = false;
        }
    }
}
