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
    GameObject shipCamObj = GameObject.Find("ShipCamera");

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

        // ปิด Main Camera
        if (Camera.main != null)
            Camera.main.gameObject.SetActive(false);

        // เปิด Ship Camera
        if (shipCamObj != null)
        {
            shipCamObj.SetActive(true);
        }
        else
        {
            Debug.LogWarning("ShipCamera not found in scene!");
        }

        // เปิดระบบควบคุมเรือ
        GetComponent<ShipController>().enabled = true;
    }

    public void ExitControlShip()
    {
        isControlling = false;
        player.gameObject.SetActive(true);
        shipCamObj.gameObject.SetActive(false);
        GetComponent<ShipController>().enabled = false;
        Debug.Log("Player exited ship control");
    }
}
