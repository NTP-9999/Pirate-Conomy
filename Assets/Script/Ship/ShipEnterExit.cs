using UnityEngine;
using System.Collections;

public class ShipEnterExit : Singleton<ShipEnterExit>
{
    [Header("References")]
    public Transform helmPoint;
    public GameObject helmUI;
    public Transform exitPoint;

    [Header("Settings")]
    public float interactRange = 3f;
    [Tooltip("ระยะเวลาในการสลับกล้อง (วินาที)")]
    public float camTransitionDuration = 1f;

    private Transform player;
    private CharacterController characterController;
    private Camera playerCamera;
    private Camera shipCamObj;
    private ShipAnchorSystem shipAnchorSystem;

    // เก็บข้อมูล parent/transform เดิมของ player
    private Transform originalParent;
    private Vector3    originalLocalPos;
    private Quaternion originalLocalRot;
    private Vector3    originalLocalScale;

    // เก็บมุมกล้องผู้เล่นก่อนขึ้นเรือ
    private Vector3    originalCamLocalPos;
    private Quaternion originalCamLocalRot;
    // เก็บมุมกล้องเรือก่อนออกเรือ
    private Transform  originalShipCamParent;
    private Vector3 originalShipCamLocalPos;
    private Quaternion originalShipCamLocalRot;

    private bool nearHelm = false;
    public bool isControlling = false;

    void Start()
    {
        // หา player และ component ต่าง ๆ
        player = GameObject.FindWithTag("Player").transform;
        characterController = player.GetComponent<CharacterController>();
        playerCamera = player.GetComponentInChildren<Camera>(true);
        shipCamObj = GetComponentInChildren<Camera>(true);
        shipAnchorSystem = GetComponent<ShipAnchorSystem>();

        helmUI.SetActive(false);
        shipCamObj.gameObject.SetActive(false);

        // เก็บ parent/transform เดิมของ player
        originalParent = player.parent;
        originalLocalPos = player.localPosition;
        originalLocalRot = player.localRotation;
        originalLocalScale = player.localScale;

        // เก็บมุมกล้องผู้เล่นในตอนเริ่ม (กันกรณีไม่ได้ขยับก่อนขึ้นเรือ)

        originalCamLocalPos = playerCamera.transform.localPosition;
        originalCamLocalRot = playerCamera.transform.localRotation;
        
        originalShipCamParent      = shipCamObj.transform.parent;
        originalShipCamLocalPos    = shipCamObj.transform.localPosition;
        originalShipCamLocalRot    = shipCamObj.transform.localRotation;
    }

    void Update()
    {
        float distToHelm = Vector3.Distance(player.position, helmPoint.position);

        // ถ้าเข้าใกล้ helm และยังไม่ได้ควบคุม
        if (distToHelm <= interactRange && !isControlling)
        {
            helmUI.SetActive(true);
            nearHelm = true;
            if (Input.GetKeyDown(KeyCode.E))
                StartControlShip();
        }
        else if (!isControlling)
        {
            helmUI.SetActive(false);
            nearHelm = false;
        }

        // ถ้ากำลังควบคุม และกด V → ออก
        if (isControlling && Input.GetKeyDown(KeyCode.V))
            ExitControlShip();

        // ระหว่างควบคุม hide UI เสมอ
        if (isControlling)
            helmUI.SetActive(false);
    }

    void StartControlShip()
    {
        // 1) เซฟมุมกล้องผู้เล่น ณ ตอนนี้
        originalCamLocalPos = playerCamera.transform.localPosition;
        originalCamLocalRot = playerCamera.transform.localRotation;

        // 2) เปลี่ยน waypoint UI ไปใช้กล้องเรือ
        WaypointUI.Instance.SetReferenceTransform(shipCamObj.transform);
        WaypointUI.Instance.SetCamera(shipCamObj);

        // 3) เริ่ม transition ไปกล้องเรือ
        StartCoroutine(SmoothSwitchToShipCam());
    }

    IEnumerator SmoothSwitchToShipCam()
    {
        // เก็บตำแหน่ง/การหมุนต้นทาง (player) และปลายทาง (shipCam)
        Vector3   startPos = playerCamera.transform.position;
        Quaternion startRot = playerCamera.transform.rotation;
        Vector3   endPos   = shipCamObj.transform.position;
        Quaternion endRot   = shipCamObj.transform.rotation;

        float elapsed = 0f;

        // -- Unparent playerCamera ชั่วคราว เพื่อไม่ให้กระทบกับ hierarchy ของ Player/Ship --
        playerCamera.transform.SetParent(null, worldPositionStays: true);

        // ให้เห็น playerCam ขณะ transition แล้วซ่อน shipCam
        playerCamera.gameObject.SetActive(true);
        shipCamObj.gameObject.SetActive(false);

        // Lerp/Slerp
        while (elapsed < camTransitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / camTransitionDuration);

            playerCamera.transform.position = Vector3.Lerp(startPos, endPos, t);
            playerCamera.transform.rotation = Quaternion.Slerp(startRot, endRot, t);

            yield return null;
        }

        // สลับเปิด shipCam และปิด playerCam
        shipCamObj.gameObject.SetActive(true);
        playerCamera.gameObject.SetActive(false);

        OnShipCamActive();
    }

    void OnShipCamActive()
    {
        // parent Player → ปิด CharacterController
        player.SetParent(transform, worldPositionStays: true);
        characterController.enabled = false;
        player.localScale = originalLocalScale;

        // วาง Player ที่ helmPoint
        player.position = helmPoint.position;
        player.rotation = helmPoint.rotation;

        // เริ่มควบคุมเรือ
        isControlling = true;
        GetComponent<ShipController>().enabled = !shipAnchorSystem.anchorDeployed;
    }

    public void ExitControlShip()
    {
        // เปลี่ยน waypoint UI กลับไป playerCam
        WaypointUI.Instance.SetReferenceTransform(player.transform);
        WaypointUI.Instance.SetCamera(playerCamera);

        StartCoroutine(SmoothSwitchToPlayerCam());
    }

    IEnumerator SmoothSwitchToPlayerCam()
    {
        // เก็บตำแหน่ง/การหมุนต้นทาง (shipCam) และปลายทาง (exitPoint)
        Vector3   startPos = shipCamObj.transform.position;
        Quaternion startRot = shipCamObj.transform.rotation;
        Vector3   endPos   = exitPoint.position;
        Quaternion endRot   = exitPoint.rotation;

        float elapsed = 0f;

        // 1) เซฟมุมกล้องเรือ ณ ตอนนี้
        originalShipCamLocalPos = shipCamObj.transform.localPosition;
        originalShipCamLocalRot = shipCamObj.transform.localRotation;
        // 2) Unparent ชั่วคราว เพื่อให้ Lerp ต้องการ world space เท่านั้น
        shipCamObj.transform.SetParent(null, worldPositionStays: true);

        // ให้เห็น shipCam ขณะ transition แล้วซ่อน playerCam
        shipCamObj.gameObject.SetActive(true);
        playerCamera.gameObject.SetActive(false);

        // Lerp/Slerp
        while (elapsed < camTransitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / camTransitionDuration);

            shipCamObj.transform.position = Vector3.Lerp(startPos, endPos, t);
            shipCamObj.transform.rotation = Quaternion.Slerp(startRot, endRot, t);

            yield return null;
        }

        // สลับปิด shipCam, เปิด playerCam
        shipCamObj.gameObject.SetActive(false);
        playerCamera.gameObject.SetActive(true);

        OnPlayerCamActive();
    }

    void OnPlayerCamActive()
    {
        // คืน parent + transform ให้ Player
        player.SetParent(originalParent, worldPositionStays: false);
        player.localPosition = originalLocalPos;
        player.localRotation = originalLocalRot;
        characterController.enabled = true;
        player.localScale = originalLocalScale;

        // คืน parent + restore มุมกล้องผู้เล่น
        playerCamera.transform.SetParent(player, worldPositionStays: false);
        playerCamera.transform.localPosition = originalCamLocalPos;
        playerCamera.transform.localRotation = originalCamLocalRot;

        // วาง Player ที่ exitPoint
        player.position = exitPoint.position;
        player.rotation = exitPoint.rotation;

        // คืน parent + restore มุมกล้องเรือ
        shipCamObj.transform.SetParent(originalShipCamParent, worldPositionStays: false);
        shipCamObj.transform.localPosition = originalShipCamLocalPos;
        shipCamObj.transform.localRotation = originalShipCamLocalRot;

        // ปิดระบบขับเรือ
        isControlling = false;
        GetComponent<ShipController>().enabled = false;
    }
}
