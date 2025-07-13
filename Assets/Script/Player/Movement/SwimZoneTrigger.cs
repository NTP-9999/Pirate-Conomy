using UnityEngine;

public class SwimZoneTrigger : MonoBehaviour
{
    [Tooltip("ชื่อ parameter ใน Animator ที่เป็น Bool สำหรับว่ายน้ำ")]
    private string swimBoolName = "isSwimming";
    private bool isPlayerInWater = false;

    public Animator playerAnimator;
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && playerAnimator != null && !isPlayerInWater)
        {
            playerAnimator.SetBool(swimBoolName, true);
            isPlayerInWater = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && playerAnimator != null && isPlayerInWater)
        {
            playerAnimator.SetBool(swimBoolName, false);
            isPlayerInWater = false;
        }
    }
}
