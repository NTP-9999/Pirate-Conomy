using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    public void Playgame()
    {
        SceneManager.LoadSceneAsync("Main");
    }
}
