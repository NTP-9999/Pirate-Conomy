using UnityEngine;
using System.Collections;

public class OilCollector : MonoBehaviour
{
    public Sprite oilIcon;   // Icon สำหรับน้ำมันใน Inventory
    public GameObject PlayerWeapon;
    public CharacterMovement movement;
    public Animator animator;
    public GameObject OilObject;
    public float collectDistance = 2f;
    public LayerMask oilLayer;
    public KeyCode collectKey = KeyCode.E;

    private bool isCollecting = false;

    void Update()
    {
        if (isCollecting) return;

        if (Input.GetKeyDown(collectKey))
        {
            TryCollect();
        }
    }

    void TryCollect()
    {
        Ray ray = new Ray(transform.position + Vector3.up, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, collectDistance, oilLayer))
        {
            OilResource oil = hit.collider.GetComponent<OilResource>();
            if (oil != null)
            {
                StartCoroutine(CollectSequence(oil));
            }
        }
    }

    public void PerformCollect(OilResource oil)
    {
        oil.Collect();
        InventoryManager.Instance.AddItem("Oil", oilIcon, 1);
    }

    public IEnumerator StartCollectFromExternal(OilResource oil)
    {
        isCollecting = true;

        movement.SetCanMove(false);
        PlayerWeapon.SetActive(false);
        OilObject.SetActive(true);

        if (animator != null)
            animator.SetTrigger("Oil");  // ใช้ animation เดิม หรือสร้างใหม่เป็น "Collect"

        yield return new WaitForSeconds(1.5f);

        PerformCollect(oil);
        OilObject.SetActive(false);
        PlayerWeapon.SetActive(true);
        movement.SetCanMove(true);
        isCollecting = false;
    }

    IEnumerator CollectSequence(OilResource oil)
    {
        isCollecting = true;

        movement.SetCanMove(false);
        PlayerWeapon.SetActive(false);
        OilObject.SetActive(true);

        if (animator != null)
            animator.SetTrigger("Oil");

        yield return new WaitForSeconds(1.5f);

        PerformCollect(oil);
        OilObject.SetActive(false);
        PlayerWeapon.SetActive(true);
        movement.SetCanMove(true);
        isCollecting = false;
    }
}
