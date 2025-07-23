using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ShipSpawnManager : MonoBehaviour
{
    private bool _skipFirstWarp = true;
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (_skipFirstWarp)
        {
            _skipFirstWarp = false;
            return;
        }
        // เรียกทันที ไม่ต้องรอเฟรมต่อไป
        PerformImmediateWarp();
    }

    private void PerformImmediateWarp()
    {
        // หา Spawn Point
        var spawnGO = GameObject.FindWithTag("ShipSpawnPoint");
        if (spawnGO == null)
        {
            Debug.LogWarning($"[ShipSpawnManager] ไม่พบ ShipSpawnPoint ใน Scene '{SceneManager.GetActiveScene().name}'");
            return;
        }

        // หาเรือ
        var ship = ShipController.Instance;
        if (ship == null) return;

        // เก็บ Collider ทุกตัว
        var colliders = ship.GetComponentsInChildren<Collider>(true);

        // ปิด Collider ทันที
        foreach (var col in colliders)
            col.enabled = false;

        // Warp ตำแหน่งและการหมุน
        Transform t = spawnGO.transform;
        Quaternion flippedRot = t.rotation * Quaternion.Euler(0f, 0f, 0f);
        ship.transform.SetPositionAndRotation(t.position, flippedRot);

        // ถ้ามี Rigidbody ก็ Warp เข้า physics
        var rb = ship.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.position = t.position;
            rb.rotation = t.rotation;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            Physics.SyncTransforms();
        }

        // สตาร์ท Coroutine เพื่อเปิด Collider คืนหลัง physics รอบถัดไป
        StartCoroutine(ReenableCollidersNextPhysicsFrame(colliders));
    }

    private IEnumerator ReenableCollidersNextPhysicsFrame(Collider[] colliders)
    {
        // รอให้ physics เรียก FixedUpdate รอบนึงก่อน
        yield return new WaitForFixedUpdate();

        // เปิด Collider คืน
        foreach (var col in colliders)
            col.enabled = true;
    }
}
