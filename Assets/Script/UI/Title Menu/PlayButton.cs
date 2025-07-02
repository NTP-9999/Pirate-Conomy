using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    public static string nextScene = "pirate"; // ชื่อซีนที่ต้องการไปหลัง loading

    public void Playgame()
    {
        SceneManager.LoadScene("Loading_screen");
    }
}
