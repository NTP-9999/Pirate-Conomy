using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class InventoryNotificationUI : MonoBehaviour
{
    [Header("UI Settings")]
    public GameObject notificationPrefab;         // Prefab ของ UI ที่ใช้แสดงข้อความ (ต้องมี TMP + CanvasGroup)
    public Transform notificationParent;          // พ่อของ Notification ทั้งหมด (ใช้ VerticalLayoutGroup)
    public float fadeOutTime = 1f;                // เวลารอก่อนค่อยๆ หายไป
    public float fadeDuration = 0.5f;               // ระยะเวลาที่ใช้จางหาย

    private Dictionary<string, NotificationEntry> activeNotifications = new Dictionary<string, NotificationEntry>();

    public void ShowNotification(string itemName, int amount)
    {
        int totalAmount = amount;

            if (activeNotifications.ContainsKey(itemName))
        {
            NotificationEntry oldEntry = activeNotifications[itemName];

            if (oldEntry.coroutine != null)
                StopCoroutine(oldEntry.coroutine);

            Destroy(oldEntry.gameObject);
            activeNotifications.Remove(itemName);
        }

        // ✅ สร้างใหม่ จากจำนวนรวมล่าสุด
        GameObject go = Instantiate(notificationPrefab, notificationParent);
        TextMeshProUGUI text = go.GetComponentInChildren<TextMeshProUGUI>();
        CanvasGroup canvasGroup = go.GetComponent<CanvasGroup>();

        text.text = $"{itemName} x{totalAmount}";
        NotificationEntry newEntry = new NotificationEntry
        {
            gameObject = go,
            text = text,
            canvasGroup = canvasGroup,
            amount = totalAmount,
            coroutine = StartCoroutine(FadeOut(go, canvasGroup, itemName))
        };

        activeNotifications[itemName] = newEntry;
    }

    private IEnumerator FadeOut(GameObject go, CanvasGroup group, string itemName)
    {
        yield return new WaitForSeconds(fadeOutTime);

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            group.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }

        Destroy(go);
        activeNotifications.Remove(itemName);
    }

    private class NotificationEntry
    {
        public GameObject gameObject;
        public TextMeshProUGUI text;
        public CanvasGroup canvasGroup;
        public int amount;
        public Coroutine coroutine;

        public void ResetTimer()
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
            }

            if (coroutine != null && gameObject != null)
            {
                InventoryNotificationUI system = gameObject.GetComponentInParent<InventoryNotificationUI>();
                if (system != null)
                {
                    system.StopCoroutine(coroutine);
                    coroutine = system.StartCoroutine(system.FadeOut(gameObject, canvasGroup, text.text.Split(' ')[0]));
                }
            }
        }
    }
}
