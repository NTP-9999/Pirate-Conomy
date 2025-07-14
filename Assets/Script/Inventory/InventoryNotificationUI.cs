using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using DG.Tweening;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class InventoryNotificationUI : MonoBehaviour
{

    [Header("UI References")]
    [SerializeField] private GameObject notificationPrefab;
    private float intervalCount=0;

    //All child in NotificationPrefab //-CanvasGroup-ItemIcon(Image)-ItemName(TextMeshPro)-Amount(TextMeshPro)
    [SerializeField] private GameObject notificationContainer;
    [SerializeField] private float displayInterval = .2f;
    [SerializeField] private float newItemDuration = 1.0f;
    [SerializeField] private float displayDuration = 2f;
    [SerializeField] private float fadeInDuration = 0.3f;
    [SerializeField] private float fadeOutDuration = 0.3f;

    private List<NotiData> notiQueue = new List<NotiData>();
    private struct NotiData
    {
        public InventoryItem item;
        public int amount;

        public NotiData(InventoryItem _item, int _amount)
        {
            item = _item;
            amount = _amount;
        }

    }
    private void Start()
    {
        if (!notificationContainer.GetComponent<VerticalLayoutGroup>())
        {
            Debug.LogWarning("NotificationContainer should have a VerticalLayoutGroup component!");
        }
    }
    private void Update()
    {
        if (notiQueue.Count > 0&&intervalCount <=0)
        {
            NotiData data = notiQueue[0];
            DisplayNotification(data.item,data.amount);
            intervalCount = displayInterval;
        }
        if (intervalCount <= 0) { }
        else if (intervalCount > 0)
        {
            intervalCount -=Time.deltaTime;
        }

    }
    public void ShowNotification(InventoryItem item, int amount)
    {
        notiQueue.Add(new NotiData(item, amount));
    }
    private void DisplayNotification(InventoryItem item, int amount)
    {
        notiQueue.RemoveAt(0);
        GameObject notification = Instantiate(notificationPrefab, notificationContainer.transform);
        NotificationDisplay notiDisplay = notification.GetComponent<NotificationDisplay>();

        notiDisplay.SetUp(item, amount);

        CanvasGroup canvasGroup = notification.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;

        DG.Tweening.Sequence sequence = DOTween.Sequence();

        // Fade in
        sequence.Append(canvasGroup.DOFade(1f, fadeInDuration));
        // Turn to white
        sequence.Append(notiDisplay.background.DOColor(Color.black, newItemDuration));
        // Wait
        sequence.AppendInterval(displayDuration);

        // Fade out
        sequence.Append(canvasGroup.DOFade(0f, fadeOutDuration));

        // Destroy after animation
        sequence.OnComplete(() => Destroy(notification));
    }
    // [Header("UI Settings")]
    // public GameObject notificationPrefab;         // Prefab ของ UI ที่ใช้แสดงข้อความ (ต้องมี TMP + CanvasGroup)
    // public Transform notificationParent;          // พ่อของ Notification ทั้งหมด (ใช้ VerticalLayoutGroup)
    // public float fadeOutTime = 1f;                // เวลารอก่อนค่อยๆ หายไป
    // public float fadeDuration = 0.5f;               // ระยะเวลาที่ใช้จางหาย

    // private Dictionary<string, NotificationEntry> activeNotifications = new Dictionary<string, NotificationEntry>();

    // public void ShowNotification(string itemName, int amount)
    // {
    //     int totalAmount = amount;

    //         if (activeNotifications.ContainsKey(itemName))
    //     {
    //         NotificationEntry oldEntry = activeNotifications[itemName];

    //         if (oldEntry.coroutine != null)
    //             StopCoroutine(oldEntry.coroutine);

    //         Destroy(oldEntry.gameObject);
    //         activeNotifications.Remove(itemName);
    //     }

    //     // ✅ สร้างใหม่ จากจำนวนรวมล่าสุด
    //     GameObject go = Instantiate(notificationPrefab, notificationParent);
    //     TextMeshProUGUI text = go.GetComponentInChildren<TextMeshProUGUI>();
    //     CanvasGroup canvasGroup = go.GetComponent<CanvasGroup>();

    //     text.text = $"{itemName} x{totalAmount}";
    //     NotificationEntry newEntry = new NotificationEntry
    //     {
    //         gameObject = go,
    //         text = text,
    //         canvasGroup = canvasGroup,
    //         amount = totalAmount,
    //         coroutine = StartCoroutine(FadeOut(go, canvasGroup, itemName))
    //     };

    //     activeNotifications[itemName] = newEntry;
    // }

    // private IEnumerator FadeOut(GameObject go, CanvasGroup group, string itemName)
    // {
    //     yield return new WaitForSeconds(fadeOutTime);

    //     float timer = 0f;
    //     while (timer < fadeDuration)
    //     {
    //         timer += Time.deltaTime;
    //         group.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
    //         yield return null;
    //     }

    //     Destroy(go);
    //     activeNotifications.Remove(itemName);
    // }

    // private class NotificationEntry
    // {
    //     public GameObject gameObject;
    //     public TextMeshProUGUI text;
    //     public CanvasGroup canvasGroup;
    //     public int amount;
    //     public Coroutine coroutine;

    //     public void ResetTimer()
    //     {
    //         if (canvasGroup != null)
    //         {
    //             canvasGroup.alpha = 1f;
    //         }

    //         if (coroutine != null && gameObject != null)
    //         {
    //             InventoryNotificationUI system = gameObject.GetComponentInParent<InventoryNotificationUI>();
    //             if (system != null)
    //             {
    //                 system.StopCoroutine(coroutine);
    //                 coroutine = system.StartCoroutine(system.FadeOut(gameObject, canvasGroup, text.text.Split(' ')[0]));
    //             }
    //         }
    //     }
    // }
}
