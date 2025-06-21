using UnityEngine;

public class ShipEnterExit : MonoBehaviour
{
    public Transform helmPoint;
    public GameObject helmUI;
    public float interactRange = 3f;

    private Transform player;
    private Camera playerCamera;
    private bool nearHelm = false;
    private bool isControlling = false;
    public bool IsControlling => isControlling;

    private GameObject shipCamObj;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // ✅ ค้นหาและเก็บกล้องของผู้เล่น
        playerCamera = player.GetComponentInChildren<Camera>(true); // true = รวมที่ถูกปิดอยู่ด้วย
        shipCamObj = GameObject.Find("ShipCamera");

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
        else if (!isControlling)
        {
            helmUI.SetActive(false);
            nearHelm = false;
        }

        if (isControlling && Input.GetKeyDown(KeyCode.V))
        {
            ExitControlShip();
        }
    }

    void StartControlShip()
    {
        if (playerCamera != null)
            playerCamera.gameObject.SetActive(false); // ❌ ปิดกล้องผู้เล่น

        if (shipCamObj != null)
        {
            shipCamObj.SetActive(true); // ✅ เปิดกล้องเรือ
        }
        else
        {
            Debug.LogWarning("ShipCamera not found in scene!");
        }

        isControlling = true;
        player.gameObject.SetActive(false);
        GetComponent<ShipController>().enabled = true;
    }

    public void ExitControlShip()
    {
        isControlling = false;

        player.gameObject.SetActive(true);
        GetComponent<ShipController>().enabled = false;

        if (shipCamObj != null)
            shipCamObj.SetActive(false); // ❌ ปิดกล้องเรือ

        if (playerCamera != null)
            playerCamera.gameObject.SetActive(true); // ✅ เปิดกล้องผู้เล่นกลับ

        Debug.Log("Player exited ship control");
    }
}
