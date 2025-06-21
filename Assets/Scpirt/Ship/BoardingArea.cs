using UnityEngine;

public class BoardingArea : MonoBehaviour
{
    public Transform boardingTargetPoint; // จุดวาร์ปขึ้นเรือ
    public GameObject pressEUI;

    private bool playerInRange = false;
    private bool hasBoarded = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            // ✅ ป้องกันการแสดง UI หากผู้เล่นเพิ่งขึ้นเรือ
            if (!hasBoarded && pressEUI != null)
                pressEUI.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            // ออกจาก Trigger แล้วสามารถขึ้นใหม่ได้
            hasBoarded = false;

            if (pressEUI != null) pressEUI.SetActive(false);
        }
    }

    void Update()
    {
        if (playerInRange && !hasBoarded && Input.GetKeyDown(KeyCode.E))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            CharacterController cc = player.GetComponent<CharacterController>();

            if (cc != null) cc.enabled = false;
            player.transform.position = boardingTargetPoint.position;
            if (cc != null) cc.enabled = true;

            hasBoarded = true;
            if (pressEUI != null) pressEUI.SetActive(false);
        }
    }
}
