using UnityEngine;
using UnityEngine.SceneManagement;

public class SmlLeviathan : MonoBehaviour
{
    public static string nextScene = "เกาะ 2";
    void Start()
    {
        GetComponent<BoxCollider>();
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            SceneManager.LoadScene("เกาะ 2");
        }
    }
}
