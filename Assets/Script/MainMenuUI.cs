using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour{
    public void Playgame(){
        SceneManager.LoadSceneAsync(1);
    }
    public void Quitgame(){
        Application.Quit();
    }
}