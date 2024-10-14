using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string gameSceneName = "Sample Scene"; // Nombre de la escena del juego

    public void PlayGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }
}
