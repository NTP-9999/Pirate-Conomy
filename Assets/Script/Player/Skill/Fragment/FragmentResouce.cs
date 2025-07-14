using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FragmentResource : MonoBehaviour
{
    [Header("Settings")]
    public string fragmentID;    // รหัสชิ้นส่วน (ต้องตรงกับ SkillManager.unlockConditions[].fragmentID)
    public int    amount = 1;    // จำนวนที่จะเก็บทีละก้อน

    [Header("UI")]
    public GameObject interactUI; // UI “Press E to collect”

    [HideInInspector] public bool playerInRange;

    void Start() {
        if (interactUI != null) interactUI.SetActive(false);
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            playerInRange = true;
            if (interactUI != null) interactUI.SetActive(true);
            var psm = other.GetComponent<PlayerStateMachine>();
            if (psm != null) psm.currentFragment = this;
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            playerInRange = false;
            if (interactUI != null) interactUI.SetActive(false);
            var psm = other.GetComponent<PlayerStateMachine>();
            if (psm != null && psm.currentFragment == this)
                psm.currentFragment = null;
        }
    }
}
