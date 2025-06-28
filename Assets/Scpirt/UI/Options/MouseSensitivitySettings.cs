using UnityEngine;
using UnityEngine.UI;

public class MouseSensitivitySettings : MonoBehaviour
{
    public Slider sensitivitySlider;
    public FirstPersonCamera cameraScript;

    void Start()
    {
        float saved = PlayerPrefs.GetFloat("MouseSensitivity", cameraScript.mouseSensitivity);
        cameraScript.mouseSensitivity = saved;
        sensitivitySlider.value = saved;
        if (cameraScript != null && sensitivitySlider != null)
        {
            // ตั้งค่า slider ให้ตรงกับ mouseSensitivity ปัจจุบัน
            sensitivitySlider.value = cameraScript.mouseSensitivity;

            // เพิ่ม event เมื่อ slider เปลี่ยนค่า
            sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
        }
    }

    void OnSensitivityChanged(float newValue)
    {
        if (cameraScript != null)
        {
            cameraScript.mouseSensitivity = newValue;
        }
        PlayerPrefs.SetFloat("MouseSensitivity", newValue);
    }
}
