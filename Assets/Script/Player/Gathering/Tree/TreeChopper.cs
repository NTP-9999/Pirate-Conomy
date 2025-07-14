using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class TreeChopper : MonoBehaviour
{
    [Header("References")]
    public GameObject axeObject;      // ขวานที่ซ่อนอยู่ก่อนใช้
    public GameObject playerWeapon;   // อาวุธปกติของผู้เล่น
    public Animator animator;

    [HideInInspector] public bool isChopping = false;

    /// <summary>
    /// เรียกจาก FSM เพื่อทำการฟันต้นไม้
    /// </summary>
    public IEnumerator StartChopFromExternal(TreeTarget tree)
    {
        isChopping = true;

        // ปิดอาวุธปกติ, เปิดขวาน
        playerWeapon.SetActive(false);
        axeObject.SetActive(true);

        // สั่งเล่นแอนิเมชัน Chop
        animator.SetTrigger("Chop");

        // รอให้แอนิเมชันเล่นจนจบ (1.5 วิ ตามตัวอย่าง)
        yield return new WaitForSeconds(1.5f);

        // ทำงานจริง: เรียก Chop บนต้นไม้ + เก็บของ
        tree.Chop();  // TreeTarget จะอัปเดต currentChops + UI + respawn
        InventoryManager.Instance.AddItem("Wood", tree.woodIcon, 1);

        // คืนสภาพอาวุธ
        axeObject.SetActive(false);
        playerWeapon.SetActive(true);

        isChopping = false;
    }
}
