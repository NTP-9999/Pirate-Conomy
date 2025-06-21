using UnityEngine;

public class ShipEnterExit : MonoBehaviour
{
    public Transform helmPoint;  // จุดพวงมาลัย
    public GameObject helmUI;    // UI ปุ่ม "กด E เพื่อควบคุมเรือ"
    public float interactRange = 3f;

    private Transform player;
    private bool nearHelm = false;
    private bool isControlling = false;
    public bool IsControlling => isControlling;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        helmUI.SetActive(false);
    }

    void Update()
    {
        float distToHelm = Vector3.Distance(player.position, helmPoint.position);
        if (distToHelm <= interactRange && !isControlling)
        {
            helmUI.SetActive(true);
            nearHelm = true;

            if (Input.GetKeyDown(KeyCode.E))
            {
                StartControlShip();
            }
        }
        else
        {
            helmUI.SetActive(false);
            nearHelm = false;
        }
    }

    void StartControlShip()
    {
        isControlling = true;
        player.gameObject.SetActive(false); // ปิดตัวผู้เล่น
        Camera.main.transform.SetParent(transform); // ย้ายกล้องตามเรือ
        Camera.main.transform.localPosition = new Vector3(0, 5, -10);
        Camera.main.transform.localEulerAngles = new Vector3(10, 0, 0);

        GetComponent<ShipController>().enabled = true;
    }

    public void ExitControlShip()
    {
        isControlling = false;
        player.gameObject.SetActive(true);
        GetComponent<ShipController>().enabled = false;
        Debug.Log("Player exited ship control");
    }
}
