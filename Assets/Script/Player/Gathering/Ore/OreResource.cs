using UnityEngine;
using System.Collections;

public class OreResource : MonoBehaviour
{
    [Header("Ore Settings")]
    public int maxHits = 5; // จำนวนครั้งที่ขุดได้ (เปลี่ยนจาก Chops เป็น Hits)
    private int currentHits = 0;

    [Header("Respawn Settings")]
    public float respawnDelay = 5f; // เวลารอเกิดใหม่ (วินาที)
    // public GameObject orePrefab; // Prefab ของแร่ (ไม่จำเป็นต้องใช้แล้วสำหรับการ respawn แบบนี้)

    [Header("UI")]
    public GameObject interactUI; // UI "Press E"

    private bool playerInRange = false;
    private MeshRenderer meshRenderer;
    private Collider oreCollider; // เปลี่ยนชื่อตัวแปรเป็น oreCollider

    private void Start()
    {
        // ดึง Collider และ MeshRenderer มาเก็บไว้ตอนเริ่มต้น
        // ตรวจสอบให้แน่ใจว่า Object นี้มี Collider ที่เหมาะสม (เช่น BoxCollider, SphereCollider)
        oreCollider = GetComponent<SphereCollider>(); 
        meshRenderer = GetComponent<MeshRenderer>();

        if (oreCollider == null)
        {
            Debug.LogWarning("OreResource: No Collider found on this GameObject.", this);
        }
        if (meshRenderer == null)
        {
            Debug.LogWarning("OreResource: No MeshRenderer found on this GameObject.", this);
        }

        if (interactUI != null)
            interactUI.SetActive(false);
    }

    private void Update()
    {
        // ตรวจสอบว่าแร่ยังมองเห็นได้และโต้ตอบได้
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && 
            (meshRenderer == null || meshRenderer.enabled) && 
            (oreCollider == null || oreCollider.enabled))
        {
            // ค้นหาสคริปต์ OreMiner (ต้องสร้างใหม่) บนผู้เล่น
            OreCollector miner = GameObject.FindWithTag("Player").GetComponent<OreCollector>();
            if (miner != null)
            {
                StartCoroutine(miner.StartMineFromExternal(this));
            }
        }
    }

    public void Hit() // เปลี่ยนชื่อเมธอดจาก Chop เป็น Hit
    {
        currentHits++;
        Debug.Log($"Ore hit: {currentHits}/{maxHits}");

        if (currentHits >= maxHits)
        {
            Debug.Log("Ore depleted!"); // เปลี่ยนข้อความเป็น "แร่หมดแล้ว"

            if (interactUI != null)
                interactUI.SetActive(false);

            StartCoroutine(RespawnOre()); // เปลี่ยนชื่อเมธอด Coroutine
            
            // ปิด MeshRenderer และ Collider ของแร่
            if (meshRenderer != null)
                meshRenderer.enabled = false;
            if (oreCollider != null)
                oreCollider.enabled = false;
        }
    }

    private IEnumerator RespawnOre() // เปลี่ยนชื่อเมธอด Coroutine
    {
        Debug.Log("Respawning ore resource...");
        yield return new WaitForSeconds(respawnDelay);

        // เปิด MeshRenderer และ Collider ของแร่กลับมา
        if (meshRenderer != null)
            meshRenderer.enabled = true;
        if (oreCollider != null)
            oreCollider.enabled = true;

        currentHits = 0; // รีเซ็ตจำนวนครั้งที่ขุดได้
        Debug.Log("Ore resource respawned and reset.");

        // หากผู้เล่นยังอยู่ในระยะหลังจากเกิดใหม่ ให้ UI แสดงขึ้นมา
        if (playerInRange && interactUI != null)
        {
            interactUI.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            // ตรวจสอบว่า MeshRenderer และ Collider เปิดอยู่ก่อนแสดง UI
            if (interactUI != null && 
                (meshRenderer == null || meshRenderer.enabled) &&
                (oreCollider == null || oreCollider.enabled))
            {
                interactUI.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (interactUI != null)
                interactUI.SetActive(false);
        }
    }
}