using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    public static string nextScene = "Main Menu"; // ชื่อซีนที่ต้องการไปหลัง loading

    public void GoMainMenu()
    {
        SceneManager.LoadScene("Loading_screen");
    }
}
