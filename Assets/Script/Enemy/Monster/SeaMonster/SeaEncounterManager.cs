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

    // 1) คำนวณทิศทางรอบๆ เรือ (แนวนอนเท่านั้น)
    Vector3 rawDir = (Random.value < 0.5f) ? shipTransform.right : -shipTransform.right;
    Vector3 spawnDir = new Vector3(rawDir.x, 0f, rawDir.z).normalized;    // ตัด Y ทิ้งให้ขนานพื้น

    // 2) คำนวณตำแหน่ง spawn XZ
    Vector3 spawnPos = shipTransform.position + spawnDir * spawnDistanceFromShip;

    // 3) กำหนด Y ให้ชิดผิวน้ำ (ถ้าน้ำอยู่ที่ y = 0 ก็ใช้ 0 แต่ถ้าน้ำสูงกว่านั้นก็แทนค่า)
    float waterY = 22f; // ← แก้ให้ตรงกับความสูงผิวน้ำในซีนคุณ
    spawnPos.y = waterY;

    // 4) สร้างมอนสเตอร์ให้หันหน้าเข้าหาเรือ
    Quaternion spawnRot = Quaternion.LookRotation(
        (shipTransform.position - spawnPos).normalized,
        Vector3.up
    );
    Debug.Log($"[SeaEncounter] ship={shipTransform.position} dir={spawnDir} spawnPos={spawnPos}");
    Vector3 eul = spawnRot.eulerAngles;
    eul.y -= 30f;
    spawnRot = Quaternion.Euler(eul);

    Instantiate(seaMonsterPrefab, spawnPos, spawnRot);

    // **Debug** ช่วยเช็ค
    Debug.Log($"🌊 Spawned sea monster at {spawnPos} with rot {spawnRot.eulerAngles}");
    Debug.DrawLine(shipTransform.position, spawnPos, Color.cyan, 5f);
}
}
