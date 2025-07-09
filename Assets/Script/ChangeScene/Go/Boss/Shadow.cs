using UnityEngine;
using UnityEngine.SceneManagement;

public class Shadow : MonoBehaviour
{
    public static string nextScene = "Shadow";
    void Start()
    {
        GetComponent<BoxCollider>();
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            SceneManager.LoadScene("Shadow");
        }
    }
}
