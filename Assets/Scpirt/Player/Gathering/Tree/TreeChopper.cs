using UnityEngine;

public class TreeChopper : MonoBehaviour
{
    [Header("Settings")]
    public KeyCode chopKey = KeyCode.E;
    public float chopDistance = 2f;
    public LayerMask treeLayer;

    [Header("References")]
    public Animator animator;
    public GameObject axeObject; // ขวานที่ซ่อนอยู่ก่อนใช้
    public GameObject PlayerWeapom;
    public CharacterMovement movement; // สคริปต์ควบคุมการเดิน

    private bool isChopping = false;

    void Update()
    {
        if (isChopping) return;

        if (Input.GetKeyDown(chopKey))
        {
            TryChop();
        }
    }

    void TryChop()
    {
        Ray ray = new Ray(transform.position + Vector3.up, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, chopDistance, treeLayer))
        {
            TreeTarget tree = hit.collider.GetComponent<TreeTarget>();
            if (tree != null)
            {
                StartCoroutine(ChopSequence(tree));
            }
        }
    }
    public System.Collections.IEnumerator StartChopFromExternal(TreeTarget tree)
    {
        isChopping = true;

        // ปิดการเดิน และเปิดขวาน
        movement.SetCanMove(false);
        PlayerWeapom.SetActive(false);
        axeObject.SetActive(true);

        // เล่นแอนิเมชัน
        if (animator != null)
            animator.SetTrigger("Chop");

        // รอให้ Animation จบ (เช่น 1.5 วิ)
        yield return new WaitForSeconds(1.5f);

        // เรียกฟังก์ชันบนต้นไม้
        tree.Chop();

        // ปิดขวานและเปิดการเดิน
        axeObject.SetActive(false);
        PlayerWeapom.SetActive(true);
        movement.SetCanMove(true);
        isChopping = false;
    }

    System.Collections.IEnumerator ChopSequence(TreeTarget tree)
    {
        isChopping = true;

        // ปิดการเดิน และเปิดขวาน
        movement.SetCanMove(false);
        PlayerWeapom.SetActive(false);
        axeObject.SetActive(true);

        // เล่นแอนิเมชัน
        if (animator != null)
            animator.SetTrigger("Chop");

        // รอให้ Animation จบ (เช่น 1.5 วิ)
        yield return new WaitForSeconds(1.5f);

        // เรียกฟังก์ชันบนต้นไม้
        tree.Chop();

        // ปิดขวานและเปิดการเดิน
        axeObject.SetActive(false);
        PlayerWeapom.SetActive(true);
        movement.SetCanMove(true);
        isChopping = false;
    }
}
