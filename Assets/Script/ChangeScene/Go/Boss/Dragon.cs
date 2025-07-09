using UnityEngine;
using UnityEngine.SceneManagement;

public class Dragon : MonoBehaviour
{
    public static string nextScene = "ice island";
    void Start()
    {
        GetComponent<BoxCollider>();
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            SceneManager.LoadScene("ice island");
        }
    }
}
