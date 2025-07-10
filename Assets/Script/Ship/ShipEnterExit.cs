using UnityEngine;

public class ShipEnterExit : MonoBehaviour
{
    public Transform helmPoint;
    public GameObject helmUI;
    public float interactRange = 3f;
    public Transform exitPoint;

    private Transform player;
    private Camera playerCamera;
    private bool nearHelm = false;
    private bool isControlling = false;
    public bool IsControlling => isControlling;

    private Camera shipCamObj;
    private ShipAnchorSystem shipAnchorSystem;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // ✅ ค้นหาและเก็บกล้องของผู้เล่น
        playerCamera = player.GetComponentInChildren<Camera>(true); // true = รวมที่ถูกปิดอยู่ด้วย
        shipCamObj = GetComponentInChildren<Camera>(true);

        helmUI.SetActive(false);
        shipAnchorSystem = GetComponent<ShipAnchorSystem>();
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
        if (isControlling)
        {
            helmUI.SetActive(false); // ❌ ปิด UI เมื่อควบคุมเรือ
        }
    }

    void StartControlShip()
    {
        WaypointUI.Instance.SetReferenceTransform(shipCamObj.transform);
        WaypointUI.Instance.SetCamera(shipCamObj);
        if (playerCamera != null)
            playerCamera.gameObject.SetActive(false); // ❌ ปิดกล้องผู้เล่น

        if (shipCamObj != null)
        {
            shipCamObj.gameObject.SetActive(true); // ✅ เปิดกล้องเรือ
        }
        else
        {
            Debug.LogWarning("ShipCamera not found in scene!");
        }

        isControlling = true;
        player.gameObject.SetActive(false);
        
            
                // *** ใช้ shipAnchorSystem ที่ Get มาใน Start() ***
                if (shipAnchorSystem != null && shipAnchorSystem.anchorDeployed)
                {
                    GetComponent<ShipController>().enabled = false;
                }
                else
                {
                    GetComponent<ShipController>().enabled = true; // ✅ เปิดการควบคุมเรือ
                }
            
    }

    public void ExitControlShip()
    {
        WaypointUI.Instance.SetReferenceTransform(player.transform);
        isControlling = false;
        WaypointUI.Instance.SetCamera(playerCamera);
        player.position = exitPoint.position;
        player.rotation = exitPoint.rotation;
        player.gameObject.SetActive(true);
        GetComponent<ShipController>().enabled = false;

        if (shipCamObj != null)
            shipCamObj.gameObject.SetActive(false); // ❌ ปิดกล้องเรือ

        if (playerCamera != null)
            playerCamera.gameObject.SetActive(true); // ✅ เปิดกล้องผู้เล่นกลับ

        Debug.Log("Player exited ship control");

        // หลังจาก player.gameObject.SetActive(true);
        BoardingArea boardingArea = FindObjectOfType<BoardingArea>();
        if (boardingArea != null)
        {
            boardingArea.CheckPlayerInArea();
        }
    }
}
