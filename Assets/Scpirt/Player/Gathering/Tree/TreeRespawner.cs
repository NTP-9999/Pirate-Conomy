using UnityEngine;
using System.Collections;

public class TreeRespawner : MonoBehaviour
{
    public static TreeRespawner Instance;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RespawnTree(GameObject treePrefab, Vector3 position, Quaternion rotation, float delay)
    {
        StartCoroutine(RespawnCoroutine(treePrefab, position, rotation, delay));
    }

    private IEnumerator RespawnCoroutine(GameObject treePrefab, Vector3 position, Quaternion rotation, float delay)
    {
        yield return new WaitForSeconds(delay);
        Instantiate(treePrefab, position, rotation);
    }
}
