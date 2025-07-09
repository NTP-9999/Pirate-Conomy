using UnityEngine;
using UnityEngine.SceneManagement;

public class SmlVerdant : MonoBehaviour
{
    public static string nextScene = "เกาะ 1";
    void Start()
    {
        GetComponent<BoxCollider>();
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            SceneManager.LoadScene("เกาะ 1");
        }
    }
}
