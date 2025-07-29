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

    public Transform player;
    public GameObject shipHUD;
    private CharacterController characterController;
    private Camera playerCamera;
    private Camera shipCamObj;
    private ShipAnchorSystem shipAnchorSystem;
    private PlayerStateMachine playerStateMachine;

    // เก็บข้อมูล parent/transform เดิมของ player
    private Transform originalParent;
    private Vector3 originalLocalPos;
    private Quaternion originalLocalRot;
    private Vector3 originalLocalScale;

    // เก็บมุมกล้องผู้เล่นก่อนขึ้นเรือ
    private Vector3 originalCamLocalPos;
    private Quaternion originalCamLocalRot;
    // เก็บมุมกล้องเรือก่อนออกเรือ
    private Transform originalShipCamParent;
    private Vector3 originalShipCamLocalPos;
    private Quaternion originalShipCamLocalRot;
    public GameObject playerHUD;
    [Header("Engine Sound")]
    [Tooltip("AudioSource ที่มีเสียงล่องเรือ (loop = true)")]
    [SerializeField] private AudioSource shipEngineAudio;
    [Tooltip("ความดังสูงสุดของเสียงเรือ")]
    [Range(0f, 1f)] public float engineMaxVolume = 1f;
    [Tooltip("ระยะเวลา Fade In/Out (วินาที)")]
    public float engineFadeDuration = 2f;

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

        originalShipCamParent = shipCamObj.transform.parent;
        originalShipCamLocalPos = shipCamObj.transform.localPosition;
        originalShipCamLocalRot = shipCamObj.transform.localRotation;
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
        playerStateMachine = player.GetComponent<PlayerStateMachine>();
        playerStateMachine.enabled = false; // ปิด StateMachine ชั่วคราว

        // เซฟมุมกล้องผู้เล่น ณ ตอนนี้
        originalCamLocalPos = playerCamera.transform.localPosition;
        originalCamLocalRot = playerCamera.transform.localRotation;

        // เปลี่ยน waypoint UI ไปใช้กล้องเรือ
        WaypointUI.Instance.SetReferenceTransform(shipCamObj.transform);
        WaypointUI.Instance.SetCamera(shipCamObj);

        StartCoroutine(SmoothSwitchToShipCam());
    }



    IEnumerator SmoothSwitchToShipCam()
    {
        // เก็บตำแหน่ง/การหมุนต้นทาง (player) และปลายทาง (shipCam)
        Vector3 startPos = playerCamera.transform.position;
        Quaternion startRot = playerCamera.transform.rotation;
        Vector3 endPos = shipCamObj.transform.position;
        Quaternion endRot = shipCamObj.transform.rotation;

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
        if (shipHUD != null)
            shipHUD.SetActive(true);

        // ไม่ย้ายตำแหน่งหรือ parent ใดๆ
        characterController.enabled = false;

        isControlling = true;
        GetComponent<ShipController>().enabled = !shipAnchorSystem.anchorDeployed;
        playerHUD.SetActive(false);

        if (shipEngineAudio != null)
            StartCoroutine(FadeEngineVolume(0f, engineMaxVolume, engineFadeDuration, playOnStart: true));
    }

    public void ExitControlShip()
    {


        WaypointUI.Instance.SetReferenceTransform(player.transform);
        WaypointUI.Instance.SetCamera(playerCamera);

        StartCoroutine(SmoothSwitchToPlayerCam());
    }

    IEnumerator SmoothSwitchToPlayerCam()
    {
        // เก็บตำแหน่ง/การหมุนต้นทาง (shipCam) และปลายทาง (exitPoint)
        Vector3 startPos = shipCamObj.transform.position;
        Quaternion startRot = shipCamObj.transform.rotation;
        Vector3 endPos = exitPoint.position;
        Quaternion endRot = exitPoint.rotation;

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
        if (playerStateMachine != null)
            playerStateMachine.enabled = true; // เปิด StateMachine กลับ
        if (shipHUD != null)
            shipHUD.SetActive(false);
        if (shipEngineAudio != null)
            StartCoroutine(FadeEngineVolume(shipEngineAudio.volume, 0f, engineFadeDuration, playOnStart: false));
        player.localScale = Vector3.one;


        characterController.enabled = true;

        // คืนกล้อง
        playerCamera.transform.SetParent(player, worldPositionStays: false);
        playerCamera.transform.localPosition = originalCamLocalPos;
        playerCamera.transform.localRotation = originalCamLocalRot;

        shipCamObj.transform.SetParent(originalShipCamParent, worldPositionStays: false);
        shipCamObj.transform.localPosition = originalShipCamLocalPos;
        shipCamObj.transform.localRotation = originalShipCamLocalRot;

        isControlling = false;
        GetComponent<ShipController>().enabled = false;

        playerHUD.SetActive(true);
    }
    /// <summary>
    /// Coroutine ปรับ volume ของ shipEngineAudio จาก from → to ในเวลา duration
    /// ถ้า playOnStart = true จะ Play() ก่อนเริ่ม fade
    /// ถ้า to == 0 และ playOnStart = false จะ Stop() หลัง fade เสร็จ
    /// </summary>
    private IEnumerator FadeEngineVolume(float from, float to, float duration, bool playOnStart)
    {
        if (shipEngineAudio == null)
            yield break;

        float elapsed = 0f;

        if (playOnStart)
        {
            shipEngineAudio.volume = 0f;
            shipEngineAudio.Play();
        }
        else
        {
            // ถ้า fade out เริ่มจากปัจจุบัน ให้ใช้ค่า volume ปัจจุบันแทน
            from = shipEngineAudio.volume;
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            shipEngineAudio.volume = Mathf.Lerp(from, to, t);
            yield return null;
        }

        shipEngineAudio.volume = to;

        if (!playOnStart && Mathf.Approximately(to, 0f))
            shipEngineAudio.Stop();
    }
}
