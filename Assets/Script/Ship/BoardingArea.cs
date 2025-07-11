using UnityEngine;

public class BoardingArea : MonoBehaviour
{
    public Transform boardingTargetPoint; // จุดวาร์ปขึ้นเรือ
    public GameObject pressEUI;

    private bool playerInRange = false;
    private bool hasBoarded = false;
    private bool canBoard = false;

    void Start()
    {
        if (pressEUI != null) pressEUI.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            canBoard = true;
            // แสดง UI เฉพาะถ้ายังไม่ได้ขึ้นเรือ
            if (pressEUI != null) pressEUI.SetActive(!hasBoarded);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            canBoard = false;
            hasBoarded = false; // รีเซ็ตแค่ตอนออก
            if (pressEUI != null) pressEUI.SetActive(false);
        }
    }

    void Update()
    {
        if (playerInRange && !hasBoarded && canBoard && Input.GetKeyDown(KeyCode.E))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            CharacterController cc = player.GetComponent<CharacterController>();

            if (cc != null) cc.enabled = false;
            player.transform.position = boardingTargetPoint.position;
            if (cc != null) cc.enabled = true;

            hasBoarded = true;
            canBoard = false;
            if (pressEUI != null) pressEUI.SetActive(false); // ซ่อน UI หลังจากกด E
        }
    }
}
