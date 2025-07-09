using UnityEngine;
using UnityEngine.SceneManagement;

public class Scorpion : MonoBehaviour
{
    public static string nextScene = "venom";
    void Start()
    {
        GetComponent<BoxCollider>();
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            SceneManager.LoadScene("venom");
        }
    }
}