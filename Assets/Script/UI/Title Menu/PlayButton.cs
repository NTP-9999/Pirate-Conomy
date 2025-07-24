using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayButton : MonoBehaviour
{
    public static string nextScene = "pirate"; // ชื่อซีนที่ต้องการไปหลัง loading

    public void Playgame()
    {
        StartCoroutine(WaitForCutScene());
    }
    IEnumerator WaitForCutScene()
    {
        yield return new WaitForSeconds(5f); 
        SceneManager.LoadScene("Loading_screen");
    }
}
