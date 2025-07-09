using UnityEngine;
using UnityEngine.SceneManagement;

public class Emberstone : MonoBehaviour
{
    public static string nextScene = "Emberstone Crag";
    void Start()
    {
        GetComponent<BoxCollider>();
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            SceneManager.LoadScene("Emberstone Crag");
        }
    }
}
