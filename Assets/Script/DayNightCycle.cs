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

    // — 1. อัพเดตเวลาและหมุนดวงอาทิตย์ เหมือนเดิม —
    currentTimeOfDay += (Time.deltaTime / secondsInFullDay) * timeMultiplier;
    if (currentTimeOfDay >= 1f) currentTimeOfDay -= 1f;
    sunLight.transform.localRotation =
        Quaternion.Euler((currentTimeOfDay * 360f) - 90f, 170f, 0);

    // — 2. คอนฟิกค่าปกติ —
    float minIntensity = 0.1f, maxIntensity = 0.3f;
    float sunriseNorm = sunRiseHour / 24f; // ex: 6/24 = 0.25
    float sunsetNorm  = sunSetHour  / 24f; // ex:18/24 = 0.75

    Color skyTint;
    float intensity;

    // — 3. ก่อนพระอาทิตย์ขึ้น (00:00 → sunrise): night → day —
    if (currentTimeOfDay < sunriseNorm)
    {
        float t = Mathf.InverseLerp(0f, sunriseNorm, currentTimeOfDay);
        skyTint    = Color.Lerp(nightSkyboxTint, daySkyboxTint, t);
        intensity  = Mathf.Lerp(minIntensity, maxIntensity, t);
    }
    // — 4. กลางวัน (sunrise → sunset): คงที่เป็น day —
    else if (currentTimeOfDay < sunsetNorm)
    {
        skyTint   = daySkyboxTint;
        intensity = maxIntensity;
    }
    // — 5. หลังพระอาทิตย์ตก (sunset → 24:00): day → night —
    else
    {
        float t = Mathf.InverseLerp(sunsetNorm, 1f, currentTimeOfDay);
        skyTint   = Color.Lerp(daySkyboxTint, nightSkyboxTint, t);
        intensity = Mathf.Lerp(maxIntensity, minIntensity, t);
    }

    // — 6. เซ็ตค่าลงในระบบจริง —
    if (skyboxMaterial != null)
        skyboxMaterial.SetColor("_Tint", skyTint);

    sunLight.intensity = intensity;

    // — 7. Ambient & Fog แล้วแต่เดิม —
    RenderSettings.ambientLight = ambientLightColor.Evaluate(currentTimeOfDay);
    RenderSettings.fogColor     = fogColor.Evaluate(currentTimeOfDay);
}

}