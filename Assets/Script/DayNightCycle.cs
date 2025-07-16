using UnityEngine;
using UnityEngine.SceneManagement;

public class DayNightCycle : MonoBehaviour
{
    [Header("Time Settings")]
    [Tooltip("ระยะเวลา 1 วันใน Unity Unit (วินาทีในเกม = 1 วัน)")]
    public float secondsInFullDay = 1440.0f; // 24 นาที = 1 วันในเกม
    [Range(0, 1)]
    [Tooltip("เวลาเริ่มต้นของวัน (0 = เที่ยงคืน, 0.25 = 6 โมงเช้า, 0.5 = เที่ยงวัน, 0.75 = 6 โมงเย็น)")]
    public float currentTimeOfDay = 0.25f; // เริ่มที่ 6 โมงเช้า
    [HideInInspector]
    public float timeMultiplier = 1f; // สามารถปรับให้เวลาเดินเร็วขึ้น/ช้าลงได้

    [Header("Light Settings")]
    public Light sunLight; // ลาก Directional Light เข้ามาใส่ใน Inspector
    public float sunRiseHour = 6f; // ชั่วโมงพระอาทิตย์ขึ้น (เช่น 6.0 = 6 โมงเช้า)
    public float sunSetHour = 18f; // ชั่วโมงพระอาทิตย์ตก (เช่น 18.0 = 6 โมงเย็น)

    [Header("Skybox & Fog Settings")]
    public Gradient ambientLightColor; // สี Ambient Light ตามช่วงเวลา (ปรับใน Inspector)
    public Gradient fogColor;          // สี Fog ตามช่วงเวลา (ปรับใน Inspector)
    public Color nightSkyboxTint = Color.grey; // สีของ Skybox ตอนกลางคืน
    public Color daySkyboxTint = Color.white;  // สีของ Skybox ตอนกลางวัน

    // สำหรับเก็บค่าเดิมของ Skybox เพื่อปรับสีได้
    private Material skyboxMaterial; 

    void Start()
    {
        if (sunLight == null)
        {
            Debug.LogError("Sun Light not assigned! Please assign your Directional Light to the 'Sun Light' field in the Inspector.");
        }

        // เก็บ Material ของ Skybox
        skyboxMaterial = RenderSettings.skybox;
        if (skyboxMaterial == null)
        {
            Debug.LogWarning("No Skybox Material assigned in Lighting Settings. Ambient light and fog may not change as expected.");
        }
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (sunLight == null)
        {
            var go = GameObject.Find("Directional Light");
            if (go != null)
                sunLight = go.GetComponent<Light>();
            else
                Debug.LogError($"DayNightCycle.OnSceneLoaded: ไม่พบ 'Directional Light' ใน Scene '{scene.name}'");
        }
    }
    void OnDestroy()
    {
        // ป้องกัน memory leak ถ้า GameObject ตัวนี้ถูกทำลาย
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Update()
    {
        if (sunLight == null) return;
        // คำนวณเวลาที่ผ่านไป
        currentTimeOfDay += (Time.deltaTime / secondsInFullDay) * timeMultiplier;
        if (currentTimeOfDay >= 1f)
        {
            currentTimeOfDay = 0f; // เมื่อครบรอบ 1 วัน ให้กลับไปเริ่มต้นใหม่
        }

        // หมุน Directional Light เพื่อจำลองการขึ้น-ตกของดวงอาทิตย์
        // 0.25 = 6 โมงเช้า, 0.5 = เที่ยงวัน, 0.75 = 6 โมงเย็น, 0 = เที่ยงคืน
        // หมุน 360 องศา ใน 1 วัน
        sunLight.transform.localRotation = Quaternion.Euler((currentTimeOfDay * 360f) - 90f, 170f, 0); // -90f เพื่อให้เริ่มจากขอบฟ้า

        // ปรับสีแสง Ambient และ Fog
        RenderSettings.ambientLight = ambientLightColor.Evaluate(currentTimeOfDay);
        RenderSettings.fogColor = fogColor.Evaluate(currentTimeOfDay);

        // ปรับความเข้มของแสงดวงอาทิตย์
        // ทำให้ดวงอาทิตย์มืดลงตอนกลางคืน
        float intensity = 0;
        if (currentTimeOfDay > 0.23f && currentTimeOfDay < 0.77f) // ช่วงกลางวัน (ประมาณ 5:30 - 18:30)
        {
            intensity = 1f; // สว่างเต็มที่
            // ปรับ Skybox เป็นสีกลางวัน
            if (skyboxMaterial != null) skyboxMaterial.SetColor("_Tint", daySkyboxTint);
        }
        else // ช่วงกลางคืน
        {
            intensity = 0.1f; // หรี่แสงลง
            // ปรับ Skybox เป็นสีกลางคืน
            if (skyboxMaterial != null) skyboxMaterial.SetColor("_Tint", nightSkyboxTint);
        }
        // สามารถใช้ AnimationCurve เพื่อควบคุมความสว่างได้ละเอียดกว่านี้
        sunLight.intensity = intensity;

    }
}