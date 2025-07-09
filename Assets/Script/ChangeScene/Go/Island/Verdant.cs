using UnityEngine;
using UnityEngine.SceneManagement;

public class Verdant : MonoBehaviour
{
    public static string nextScene = "Verdant Timberhold";
    void Start()
    {
        GetComponent<BoxCollider>();
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            SceneManager.LoadScene("Verdant Timberhold");
        }
    }
}
