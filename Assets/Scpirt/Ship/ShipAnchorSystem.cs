using UnityEngine;
using UnityEngine.UI;

public class ShipAnchorSystem : MonoBehaviour
{
    [Header("Anchor Settings")]
    public float holdTimeToTrigger = 2f; // เวลาที่ต้อง Hold ปุ่ม G
    public float anchorSinkDuration = 2f; // เวลาจมสมอ
    public float anchorLiftDuration = 2f; // เวลาดึงกลับ
    public float dragForce = 10f; // แรงดึงกลับเมื่อปล่อยสมอในขณะเคลื่อนที่

    [Header("UI References")]
    public GameObject anchorUI;
    public Image gImage;
    public Image progressBar;
    [Header("Drift Settings")]
    public float driftForce = 1f;   
    private float holdTimer = 0f;
    private bool isInAnchorZone = false;
    private bool isHolding = false;
    private bool anchorDeployed = false;
    private bool isWaitingAnchorSettle = false;

    private Rigidbody shipRb;

    void Start()
    {
        shipRb = GetComponent<Rigidbody>();
        anchorUI.SetActive(false);
        progressBar.fillAmount = 0f;
    }

    void Update()
    {
        if (!isInAnchorZone) return;

        if (!anchorDeployed && !isWaitingAnchorSettle && Input.GetKey(KeyCode.G))
        {
            HoldAnchorKey(() => StartCoroutine(DeployAnchor()));
        }
        else if (anchorDeployed && !isWaitingAnchorSettle && Input.GetKey(KeyCode.G))
        {
            HoldAnchorKey(() => StartCoroutine(RetractAnchor()));
        }
        else if (!Input.GetKey(KeyCode.G))
        {
            holdTimer = 0f;
            progressBar.fillAmount = 0f;
        }
        HandleDrift();
    }
    void HandleDrift()
    {
        // ถ้าไม่ได้ deploy สมอ + ไม่มีคนควบคุม → ให้ไหล
        bool isControlled = GetComponent<ShipController>().enabled;

        if (!anchorDeployed && !isControlled)
        {
            // เพิ่มแรงไปตามทิศทางเรือเล็ก ๆ
            Vector3 driftDirection = transform.forward; // หรือ random เล็กน้อยก็ได้
            shipRb.AddForce(driftDirection * driftForce * Time.deltaTime, ForceMode.Acceleration);
        }
    }

    void HoldAnchorKey(System.Action onComplete)
    {
        holdTimer += Time.deltaTime;
        progressBar.fillAmount = Mathf.Clamp01(holdTimer / holdTimeToTrigger);

        if (holdTimer >= holdTimeToTrigger)
        {
            anchorUI.SetActive(false);
            holdTimer = 0f;
            progressBar.fillAmount = 0f;
            onComplete?.Invoke();
        }
    }

    System.Collections.IEnumerator DeployAnchor()
    {
        isWaitingAnchorSettle = true;
        CharacterMovement.Instance.SetCanMove(false);


        // สมอเริ่มจม
        float t = 0f;
        while (t < anchorSinkDuration)
        {
            t += Time.deltaTime;
            yield return null;
        }

        anchorDeployed = true;
        isWaitingAnchorSettle = false;
        GetComponent<ShipController>().enabled = false;

        // ดึงกลับนิดหน่อยถ้ามีความเร็ว
        Vector3 horizontalVel = new Vector3(shipRb.linearVelocity.x, 0, shipRb.linearVelocity.z);
        if (horizontalVel.magnitude > 0.5f)
        {
            Vector3 reverseForce = -horizontalVel.normalized * dragForce;
            shipRb.AddForce(reverseForce, ForceMode.Impulse);
        }

        // หยุดการเคลื่อนที่
        shipRb.linearVelocity = Vector3.zero;
        shipRb.angularVelocity = Vector3.zero;
        CharacterMovement.Instance.SetCanMove(true);

        // แสดง UI ดึงกลับ
        anchorUI.SetActive(true);
    }

    System.Collections.IEnumerator RetractAnchor()
    {
        isWaitingAnchorSettle = true;
        CharacterMovement.Instance.SetCanMove(false);
        float t = 0f;
        while (t < anchorLiftDuration)
        {
            t += Time.deltaTime;
            yield return null;
        }

        anchorDeployed = false;
        isWaitingAnchorSettle = false;
        GetComponent<ShipController>().enabled = true;
        CharacterMovement.Instance.SetCanMove(true);
        // แสดง UI ปล่อยสมออีกครั้ง
        anchorUI.SetActive(true);
    }

    public void SetAnchorZoneState(bool state)
    {
        isInAnchorZone = state;

        if (state && !isWaitingAnchorSettle)
        {
            anchorUI.SetActive(true);
        }
        else
        {
            anchorUI.SetActive(false);
            holdTimer = 0f;
            progressBar.fillAmount = 0f;
        }
    }
}
