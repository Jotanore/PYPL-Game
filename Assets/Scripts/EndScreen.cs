using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreen : MonoBehaviour
{
    public string menuSceneName = "MainMenu";
    public string gameSceneName = "Sample Scene";// Nombre de la escena del juego

    public void Menu()
    {
        SceneManager.LoadScene(menuSceneName);
    }

    public void Play()
    {
        SceneManager.LoadScene(gameSceneName);
    }
}
