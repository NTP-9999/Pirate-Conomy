using UnityEngine;
using UnityEngine.SceneManagement;

public class Leviathan : MonoBehaviour
{
    public static string nextScene = "Leviathan’s Grave";
    void Start()
    {
        GetComponent<BoxCollider>();
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            SceneManager.LoadScene("Leviathan’s Grave");
        }
    }
}
