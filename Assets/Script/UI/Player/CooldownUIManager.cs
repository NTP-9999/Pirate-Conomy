using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class CooldownUIManager : MonoBehaviour
{
    public static CooldownUIManager Instance;

    [Tooltip("ลาก Prefab CooldownIcon มาที่นี่")]
    public GameObject iconPrefab;
    [Tooltip("ระยะห่างระหว่างไอคอน (px)")]
    public float iconSpacing = 16f;

    // เก็บข้อมูล icon ที่กำลัง active
    List<CooldownIconUI> activeIcons = new List<CooldownIconUI>();

    void Awake()
    {
        // singleton
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// สร้าง icon ใหม่พร้อมนับ cooldown
    /// </summary>
    /// <param name="iconSprite">Sprite ของสกิล</param>
    /// <param name="duration">เวลาคูลดาวน์</param>
    public void ShowCooldown(Sprite iconSprite, float duration)
    {
        // Instantiate prefab
        var go = Instantiate(iconPrefab, transform);
        var iconUI = go.GetComponent<CooldownIconUI>();
        // เริ่มนับและลบตัวเองตอนจบ
        iconUI.Initialize(iconSprite, duration, () =>
        {
            activeIcons.Remove(iconUI);
            Destroy(go);
            RepositionIcons();
        });
        activeIcons.Add(iconUI);
        RepositionIcons();
    }

    // จัดตำแหน่งซ้ายต่อเนื่องจากขวาให้ทุก icon
    void RepositionIcons()
    {
        for (int i = 0; i < activeIcons.Count; i++)
        {
            var rt = activeIcons[i].GetComponent<RectTransform>();

            // สมมติ Container ถูกตั้ง Anchor & Pivot ให้เป็น Top‑Right
            // เราจะวาง Icon แรกที่ (0,0) = มุมขวาบนของ Container
            // ไอคอนถัดไป ให้เลื่อนลง (ลบค่า Y) ตามความสูง+spacing
            float x = 0f;
            float y = - i * (rt.sizeDelta.y + iconSpacing);

            rt.anchoredPosition = new Vector2(x, y);
        }
    }
}
