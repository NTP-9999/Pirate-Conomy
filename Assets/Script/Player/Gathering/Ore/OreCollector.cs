using UnityEngine;
using System.Collections;

public class OreCollector : MonoBehaviour
{
    public Sprite oreIcon;
    public GameObject playerWeapon, pickaxeObject;
    public Animator animator;
    [HideInInspector] public bool isMining;

    public IEnumerator StartMineFromExternal(OreResource ore)
    {
        isMining = true;
        playerWeapon.SetActive(false);
        pickaxeObject.SetActive(true);
        animator.SetTrigger("Mine");
        ore.interactUI.Interacting();
        yield return new WaitForSeconds(1.5f);
        ore.Hit();
        InventoryManager.Instance.AddItem("Ore", oreIcon, 1);
        pickaxeObject.SetActive(false);
        playerWeapon.SetActive(true);
        isMining = false;
    }
}
