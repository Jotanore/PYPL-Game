using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string gameSceneName = "Sample Scene"; // Nombre de la escena del juego
    public bool pvp = true;
    public void PlayGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }
}
