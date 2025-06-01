using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Playgame()
    {
        SceneManager.LoadSceneAsync("Main");
    }
    public void Quitgame()
    {
        Application.Quit();
    }
}