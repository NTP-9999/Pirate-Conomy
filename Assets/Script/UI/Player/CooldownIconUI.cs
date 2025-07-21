using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

[RequireComponent(typeof(RectTransform))]
public class CooldownIconUI : MonoBehaviour
{
    [Header("References")]
    public Image   iconImage;     // drag IconImage
    public Image   fillOverlay;   // drag FillOverlay
    public TextMeshProUGUI timerText; // drag TimerText

    float duration;
    float remaining;
    Action onComplete;

    /// <summary>
    /// เซ็ต sprite, เวลาคูลดาวน์ และ callback ตอนจบ
    /// </summary>
    public void Initialize(Sprite sprite, float cooldown, Action onComplete)
    {
        iconImage.sprite = sprite;
        duration   = cooldown;
        remaining  = cooldown;
        this.onComplete = onComplete;

        fillOverlay.fillAmount = 0f;
        timerText.text = Mathf.Ceil(remaining).ToString();
    }

    void Update()
    {
        remaining -= Time.deltaTime;
        if (remaining <= 0f)
        {
            onComplete?.Invoke();
            return;
        }
        // อัพเดต radial fill (0→1 ตาม % จบ)
        fillOverlay.fillAmount = (duration - remaining) / duration;
        // อัพเดตตัวเลขวินาที
        timerText.text = Mathf.Ceil(remaining).ToString();
    }
}
