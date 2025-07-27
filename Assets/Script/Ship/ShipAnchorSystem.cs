using UnityEngine;
using UnityEngine.UI;

public class ShipAnchorSystem : MonoBehaviour
{
    [Header("Anchor Settings")]
    public float holdTimeToTrigger = 2f;
    public float anchorSinkDuration = 2f;
    public float anchorLiftDuration = 2f;
    public float dragForce = 10f;

    [Header("UI References")]
    public GameObject anchorUI;
    public Image gImage;
    public Image progressBar;

    [Header("Drift Settings")]
    public float driftForce = 1f;

    private float holdTimer = 0f;
    private bool isInAnchorZone = false;
    private bool isHolding = false;
    public bool anchorDeployed = false;
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
        bool isControlled = GetComponent<ShipController>().enabled;

        if (!anchorDeployed && !isControlled)
        {
            Vector3 driftDirection = transform.forward;
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

        // ถ้ามีระบบควบคุมอื่น เช่น disable input, ทำตรงนี้แทน
        // Example: inputController.SetEnabled(false);

        float t = 0f;
        while (t < anchorSinkDuration)
        {
            t += Time.deltaTime;
            yield return null;
        }

        anchorDeployed = true;
        isWaitingAnchorSettle = false;
        GetComponent<ShipController>().enabled = false;

        Vector3 horizontalVel = new Vector3(shipRb.linearVelocity.x, 0, shipRb.linearVelocity.z);
        if (horizontalVel.magnitude > 0.5f)
        {
            Vector3 reverseForce = -horizontalVel.normalized * dragForce;
            shipRb.AddForce(reverseForce, ForceMode.Impulse);
        }

        shipRb.linearVelocity = Vector3.zero;
        shipRb.angularVelocity = Vector3.zero;

        anchorUI.SetActive(true);
    }

    System.Collections.IEnumerator RetractAnchor()
    {
        isWaitingAnchorSettle = true;

        float t = 0f;
        while (t < anchorLiftDuration)
        {
            t += Time.deltaTime;
            yield return null;
        }

        anchorDeployed = false;
        isWaitingAnchorSettle = false;
        GetComponent<ShipController>().enabled = true;

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
