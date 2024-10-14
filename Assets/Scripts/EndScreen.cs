using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreen : MonoBehaviour
{
    public string gameSceneName = "MainMenu"; // Nombre de la escena del juego

    public void PlayGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }
}
