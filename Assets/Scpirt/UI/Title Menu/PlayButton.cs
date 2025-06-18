using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

public class PlayButton : MonoBehaviour
{
    public void Playgame()
    {
        SceneManager.LoadSceneAsync("pirate");
    }
}
