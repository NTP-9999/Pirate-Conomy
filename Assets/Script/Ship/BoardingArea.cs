using UnityEngine;

public class BoardingArea : MonoBehaviour
{
    public Transform boardingTargetPoint; // จุดวาร์ปขึ้นเรือ
    public GameObject pressEUI;

    private bool playerInRange = false;
    private bool hasBoarded = false;
    private bool canBoard = false; // อนุญาตให้กด E ได้เฉพาะหลังจากเดินเข้ามา
    private bool hasEverExited = false;

    void Start()
    {
        pressEUI.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            // อนุญาตให้กด E เฉพาะถ้าเคยออกจาก BoardingArea แล้ว
            canBoard = hasEverExited;
            pressEUI.SetActive(canBoard);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            hasBoarded = false;
            canBoard = false;
            hasEverExited = true; // เคยออกจาก BoardingArea แล้ว
            if (pressEUI != null) pressEUI.SetActive(false);
        }
    }

    void Update()
    {
        // เพิ่มเช็ค canBoard ด้วย
        if (playerInRange && !hasBoarded && canBoard && Input.GetKeyDown(KeyCode.E))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            CharacterController cc = player.GetComponent<CharacterController>();

            if (cc != null) cc.enabled = false;
            player.transform.position = boardingTargetPoint.position;
            if (cc != null) cc.enabled = true;

            hasBoarded = true;
            canBoard = false; // ต้องเดินออกแล้วเข้ามาใหม่ถึงจะกด E ได้อีก
            if (pressEUI != null) pressEUI.SetActive(false);
        }
    }

    public void CheckPlayerInArea()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Collider area = GetComponent<Collider>();
        if (area != null)
        {
            // ใช้ OverlapSphere ที่ตำแหน่ง player
            Collider[] hits = Physics.OverlapSphere(player.transform.position, 0.1f);
            bool found = false;
            foreach (var hit in hits)
            {
                if (hit == area)
                {
                    found = true;
                    break;
                }
            }
            playerInRange = found;
            // ตัวแปร canBoard จะถูกตั้งค่าใหม่ใน OnTriggerEnter และ OnTriggerExit
            // ดังนั้นตรวจสอบว่า playerInRange เป็น true และ hasBoarded เป็น false
            if (playerInRange && !hasBoarded && pressEUI != null)
                pressEUI.SetActive(true);
            else if (!playerInRange && pressEUI != null)
                pressEUI.SetActive(false);
        }
    }
}
