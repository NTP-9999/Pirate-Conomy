using UnityEngine;
using System.Collections.Generic;

public class ShopKeeper : MonoBehaviour
{
    public GameObject shopUIPanel;  
    public ShopManager shopManager; 
    private bool canOpenShop = false; 
    private bool playerInShopRange = false;
    public bool shopAlwaysOpen = false;
    public GameObject pressEUI;
    public PlayerController playerController;
    public PlayerSkillController playerSkillController;

    void Start()
    {
        if (playerController == null)
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                playerController = playerObj.GetComponent<PlayerController>();
                
        }
        if (playerSkillController == null)
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                playerSkillController = playerObj.GetComponent<PlayerSkillController>();
        }
        if (shopUIPanel != null) shopUIPanel.SetActive(false);
        if (shopAlwaysOpen)     canOpenShop = true;
    }

    void Update()
    {
        if (canOpenShop && playerInShopRange && Input.GetKeyDown(KeyCode.E))
            OpenShop();
    }

    public void OpenShop()
    {
        // เปิด UI ร้าน
        if (shopUIPanel != null) shopUIPanel.SetActive(true);
        shopManager?.OpenShop();

        // ปิดการควบคุมผู้เล่น
        if (playerController != null)
        {
            playerController.enabled = false;  // Disabled ทั้งสคริปต์
            playerController.canMove = false;  // หรือถ้าต้องการแค่ล็อกเดินก็ใช้ค่านี้ได้
        }
        if (playerSkillController != null)
            playerSkillController.enabled = false;


        // เอาเมาส์มาปลดล็อก และหยุดเวลา
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;
        Time.timeScale   = 0f;
    }

    public void CloseShop()
    {
        // คืนเวลา
        Time.timeScale = 1f;

        // ปิด UI ร้าน
        if (shopUIPanel != null) shopUIPanel.SetActive(false);

        // เปิดการควบคุมผู้เล่นคืน
        if (playerController != null)
        {
            playerController.enabled = true;
            playerController.canMove = true;
        }
        if (playerSkillController != null)
            playerSkillController.enabled = true;

        // ล็อกเมาส์กลับและซ่อน
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInShopRange = true;
            pressEUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInShopRange = false;
            pressEUI.SetActive(false);
        }
    }

    public void EnableShop()
    {
        canOpenShop = true;
        // (ถ้าต้องการให้เดินได้ทันทีหลังรับเควส สามารถใส่ playerController.canMove = true; ได้ที่นี่)
    }
}
