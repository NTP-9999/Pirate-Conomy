using UnityEngine;

public class SeaEncounterManager : MonoBehaviour
{
    public GameObject seaMonsterPrefab;
    public Transform shipTransform;

    [Header("Spawn Settings")]
    public float spawnChancePerSecond = 0.05f;
    public float cooldownDuration = 60f;
    public float spawnDistanceFromShip = 20f;
    private float timer = 0f;

    private float lastSpawnTime = -Mathf.Infinity;

    void Update()
    {
        if (!ShipEnterExit.Instance || !ShipEnterExit.Instance.isControlling)
            return;

        if (Time.time < lastSpawnTime + cooldownDuration)
            return;

        timer += Time.deltaTime;

        if (timer >= 1f) // ทุก ๆ 1 วินาที
        {
            timer = 0f;

            if (Random.value < spawnChancePerSecond)
            {
                SpawnSeaMonster();
                lastSpawnTime = Time.time;
            }
        }
    }

    void SpawnSeaMonster()
    {
        if (seaMonsterPrefab == null || shipTransform == null)
        {
            Debug.LogWarning("Missing prefab or shipTransform.");
            return;
        }

        // สุ่มซ้าย/ขวาเรือ
        Vector3 spawnDir = (Random.value < 0.5f) ? shipTransform.right : -shipTransform.right;
        Vector3 spawnPos = shipTransform.position + spawnDir * spawnDistanceFromShip;

        Instantiate(seaMonsterPrefab, spawnPos, Quaternion.identity);
        Debug.Log("🌊 Sea monster spawned!");
    }
}
