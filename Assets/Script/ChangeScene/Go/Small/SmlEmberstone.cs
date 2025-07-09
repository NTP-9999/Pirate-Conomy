using UnityEngine;
using UnityEngine.SceneManagement;

public class SmlEmberstone : MonoBehaviour
{
    public static string nextScene = "เกาะ 3";
    void Start()
    {
        GetComponent<BoxCollider>();
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            SceneManager.LoadScene("เกาะ 3");
        }
    }
}
