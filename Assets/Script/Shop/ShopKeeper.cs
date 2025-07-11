using UnityEngine;

public class ShopKeeper : MonoBehaviour
{
    public GameObject shopUIPanel;  // UI ร้านค้าใน Scene
    public ShopManager shopManager; // อ้างอิง ShopManager ของร้านนี้
    private bool canOpenShop = false; // Flag ว่าร้านค้า "พร้อมให้เปิด" (ได้หลังจากเควส)
    private bool playerInShopRange = false; // ตอนนี้ผู้เล่นอยู่ในระยะร้านค้าหรือเปล่า
    public bool shopAlwaysOpen = false;
    public GameObject pressEUI;

    void Start()
    {
        if (shopUIPanel != null)
            shopUIPanel.SetActive(false);
        if (shopAlwaysOpen)
        {
            canOpenShop = true;
        }
    }

    public void EnableShop()
    {
        canOpenShop = true;
    }

    private void Update()
    {
        // ✅ เปิดร้านได้เฉพาะตอนที่:
        // - ร้านเปิดใช้งานแล้ว (EnableShop เคยถูกเรียก)
        // - ผู้เล่นอยู่ในระยะ (playerInShopRange)
        if (canOpenShop && playerInShopRange && Input.GetKeyDown(KeyCode.E))
        {
            OpenShop();
        }
    }

    public void OpenShop()
    {
        if (shopUIPanel != null)
            shopUIPanel.SetActive(true);

        if (shopManager != null)
            shopManager.OpenShop();

        // ปิดทุก Component (ยกเว้น Transform) ของ GameObject ที่ Tag เป็น "Player"
        GameObject mc = GameObject.FindGameObjectWithTag("Player");
        if (mc != null)
        {
            foreach (var comp in mc.GetComponents<MonoBehaviour>())
            {
                comp.enabled = false;
            }
            // ถ้ามี Component ที่ไม่ใช่ MonoBehaviour (เช่น Collider, Renderer) ให้ปิดด้วย
            foreach (var comp in mc.GetComponents<Component>())
            {
                if (!(comp is Transform) && !(comp is MonoBehaviour))
                {
                    if (comp is Behaviour behaviour)
                        behaviour.enabled = false;
                    else if (comp is Collider collider)
                        collider.enabled = false;
                    else if (comp is Renderer renderer)
                        renderer.enabled = false;
                    // เพิ่มประเภทอื่น ๆ ตามต้องการ
                }
            }
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        CharacterMovement.Instance.SetCanMove(false);
        Time.timeScale = 0f;
    }

    public void CloseShop()
    {
        Time.timeScale = 1f;
        if (shopUIPanel != null)
            shopUIPanel.SetActive(false);

        // เปิด Component กลับ
        GameObject mc = GameObject.FindGameObjectWithTag("Player");
        if (mc != null)
        {
            foreach (var comp in mc.GetComponents<MonoBehaviour>())
            {
                comp.enabled = true;
            }
            foreach (var comp in mc.GetComponents<Component>())
            {
                if (!(comp is Transform) && !(comp is MonoBehaviour))
                {
                    if (comp is Behaviour behaviour)
                        behaviour.enabled = true;
                    else if (comp is Collider collider)
                        collider.enabled = true;
                    else if (comp is Renderer renderer)
                        renderer.enabled = true;
                }
            }
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        CharacterMovement.Instance.SetCanMove(true);
    }

    // 🎯 เพิ่ม OnTriggerEnter/Exit
    private void OnTriggerEnter(Collider other)
    {
        pressEUI.SetActive(true);
        if (other.CompareTag("Player"))
        {
            playerInShopRange = true;
            Debug.Log("Player entered shop range");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        pressEUI.SetActive(false);
        if (other.CompareTag("Player"))
        {
            playerInShopRange = false;
            Debug.Log("Player exited shop range");
        }
    }
}
