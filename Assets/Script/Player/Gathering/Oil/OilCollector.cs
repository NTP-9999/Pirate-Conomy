using UnityEngine;
using System.Collections;

public class OilCollector : MonoBehaviour
{
    public Sprite  oilIcon;
    public GameObject playerWeapon, oilObject;
    public Animator animator;
    [HideInInspector] public bool isCollecting;

    public IEnumerator StartCollectFromExternal(OilResource oil)
    {
        isCollecting = true;
        playerWeapon.SetActive(false);
        oilObject.SetActive(true);
        animator.SetTrigger("Oil");
        yield return new WaitForSeconds(1.5f);
        oil.Collect();
        InventoryManager.Instance.AddItem("Oil", oilIcon, 1);
        oilObject.SetActive(false);
        playerWeapon.SetActive(true);
        isCollecting = false;
    }
}
