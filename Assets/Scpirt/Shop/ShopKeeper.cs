using UnityEngine;

public class ShopKeeper : MonoBehaviour
{
    public GameObject shopUIPanel;  // UI ร้านค้าใน Scene
    private bool canOpenShop = false; // Flag ว่าตอนนี้ร้านเปิดได้ไหม

    void Start()
    {
        if (shopUIPanel != null)
            shopUIPanel.SetActive(false);
    }

    public void EnableShop()
    {
        canOpenShop = true;
    }

    private void Update()
    {
        if (canOpenShop && Input.GetKeyDown(KeyCode.E))
        {
            OpenShop();
        }
    }

    public void OpenShop()
    {
        if (shopUIPanel != null)
            shopUIPanel.SetActive(true);

        // ปิดการเดิน & เปิดเมาส์
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        CharacterMovement.Instance.SetCanMove(false);
    }

    public void CloseShop()
    {
        if (shopUIPanel != null)
            shopUIPanel.SetActive(false);

        // กลับมาเดินได้ & ล็อกเมาส์
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        CharacterMovement.Instance.SetCanMove(true);
    }
}
