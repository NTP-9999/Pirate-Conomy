using UnityEngine;
using System;
using System.Collections.Generic;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance { get; private set; }

    [Serializable]
    public struct Condition {
        public string fragmentID;
        public int    requiredCount;
        public string skillName;
    }
    public Condition[] unlockConditions;

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
                Unlock(c.skillName);
            }
        }
    }

    private void Unlock(string skillName) {
        unlockedSkills.Add(skillName);
        Debug.Log($"<color=yellow>Skill Unlocked:</color> {skillName}");
        // TODO: ส่ง event ให้ UI แสดง popup
    }

    public bool IsUnlocked(string skillName)
        => unlockedSkills.Contains(skillName);
}
