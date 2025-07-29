using UnityEngine;
using System;
using System.Collections.Generic;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance { get; private set; }

    [Serializable]
    public struct Condition
    {
        public string   fragmentID;
        public int      requiredCount;
        public string   skillName;
        public GameObject manualUI;
    }
    public FirstPersonCamera fps;
    public FirstPersonCamera uicam;
    public PlayerStateMachine ps;
    public PlayerSkillController psc;

    public Condition[] unlockConditions;
    public event Action<string> OnSkillUnlocked;

    private HashSet<string> unlockedSkills = new HashSet<string>();

    void Awake() {
        if (Instance != null) Destroy(gameObject);
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // เรียกเมื่อจำนวน Fragment เปลี่ยน
    public void TryUnlockSkill(string fragmentID, int currentCount) {
        foreach (var c in unlockConditions) {
            if (c.fragmentID == fragmentID
             && currentCount >= c.requiredCount
             && !unlockedSkills.Contains(c.skillName))
            {
                Unlock(c);  // เราส่งทั้ง Condition เข้ามา
            }
        }
    }

    // เปลี่ยน signature ให้รับ Condition แทน string
    private void Unlock(Condition c) {
        unlockedSkills.Add(c.skillName);
        Debug.Log($"<color=yellow>Skill Unlocked:</color> {c.skillName}");

        // ถ้า Inspector มีโยง Manual UI มา ก็เปิดมัน
        if (c.manualUI != null)
        {
            c.manualUI.SetActive(true);
            PlayerAudioManager.Instance.PlayOneShot(PlayerAudioManager.Instance.unlockSkillSound);
            fps.enabled = false; // ปิดกล้องชั่วคราว
            uicam.enabled = false; // เปิดกล้อง UI
            ps.enabled = false; // ปิด State Machine ชั่วคราว
            psc.enabled = false; // ปิด Skill Controller ชั่วคราว
        }

        Cursor.visible = true;
    Cursor.lockState = CursorLockMode.None;


        // ส่ง event ให้คนอื่นรู้ว่าสกิลนี้ปลดล็อคแล้ว
        OnSkillUnlocked?.Invoke(c.skillName);
    }
    public void CloseManualUI()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        fps.enabled = true; // เปิดกล้องกลับ
        uicam.enabled = true; // ปิดกล้อง UI
        ps.enabled = true; // เปิด State Machine กลับ
        psc.enabled = true; // เปิด Skill Controller กลับ
    }

    public bool IsUnlocked(string skillName)
        => unlockedSkills.Contains(skillName);
}
