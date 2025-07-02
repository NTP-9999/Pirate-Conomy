using UnityEngine;
using UnityEngine.UI;

public class MouseSensitivitySettings : MonoBehaviour
{
    public Slider sensitivitySlider;
    public FirstPersonCamera cameraScript;

    void Start()
    {
        float saved;
        if (PlayerPrefs.HasKey("MouseSensitivity"))
        {
            saved = PlayerPrefs.GetFloat("MouseSensitivity");
        }
        else
        {
            saved = 270f; // default กลางๆ
            PlayerPrefs.SetFloat("MouseSensitivity", saved); // บันทึกไว้
        }

        cameraScript.mouseSensitivity = saved;
        sensitivitySlider.value = saved;

        sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
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
