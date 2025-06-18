using UnityEngine;

public class TestObjectiveSetter : MonoBehaviour
{
    [Tooltip("Drag the GameObject you want to be the objective target here.")]
    public Transform myObjectiveTarget; // ลาก GameObject เป้าหมายมาใส่ใน Inspector

    [Header("Test Controls")]
    public KeyCode setObjectiveKey = KeyCode.Alpha1; // ปุ่ม 1
    public KeyCode clearObjectiveKey = KeyCode.Alpha2; // ปุ่ม 2

    void Update()
    {
        // ตรวจสอบว่า ObjectiveManager พร้อมใช้งาน (Singleton)
        if (ObjectiveManager.Instance == null)
        {
            // Debug.LogError("TestObjectiveSetter: ObjectiveManager.Instance is null. Is ObjectiveManager set up correctly in the scene and initialized?", this);
            return; // หยุดการทำงานถ้า ObjectiveManager ยังไม่พร้อม
        }

        if (Input.GetKeyDown(setObjectiveKey))
        {
            if (myObjectiveTarget != null)
            {
                ObjectiveManager.Instance.SetNewObjective(myObjectiveTarget, "Find the ancient artifact!");
            }
            else
            {
                Debug.LogError("TestObjectiveSetter: myObjectiveTarget is not assigned. Please assign it in the Inspector to set an objective.", this);
            }
        }

        if (Input.GetKeyDown(clearObjectiveKey))
        {
            ObjectiveManager.Instance.ClearObjective();
        }
    }
}