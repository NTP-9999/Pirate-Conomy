using UnityEngine;
using UnityEngine.UI; // สำหรับ Slider
using UnityEngine.Audio; // สำหรับ AudioMixer

public class VolumeSettings : MonoBehaviour
{
    [Header("Audio References")]
    [Tooltip("Drag your MainAudioMixer here from the Project window.")]
    [SerializeField] private AudioMixer mainMixer; // อ้างอิงถึง Audio Mixer ของเรา

    [Header("UI References")]
    [Tooltip("Drag the MasterVolumeSlider UI element here from the Hierarchy.")]
    [SerializeField] private Slider masterVolumeSlider; // อ้างอิงถึง UI Slider สำหรับ Master Volume

    private const string MASTER_VOLUME_PARAM = "MasterVolume"; // ชื่อของ Exposed Parameter ใน Audio Mixer

    void Awake()
    {
        // ตรวจสอบว่าได้ลาก Reference มาใส่ครบถ้วนหรือไม่
        if (mainMixer == null)
        {
            Debug.LogError("VolumeSettings: mainMixer is not assigned! Please assign it in the Inspector.", this);
            enabled = false;
            return;
        }
        if (masterVolumeSlider == null)
        {
            Debug.LogError("VolumeSettings: masterVolumeSlider is not assigned! Please assign it in the Inspector.", this);
            enabled = false;
            return;
        }

        // โหลดค่า Volume ที่บันทึกไว้เมื่อเริ่มต้น
        LoadVolume();

        // เพิ่ม Listener ให้กับ Slider เพื่อให้เมธอด SetMasterVolume ถูกเรียกเมื่อค่า Slider เปลี่ยน
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
    }

    /// <summary>
    /// ตั้งค่า Master Volume จากค่าของ Slider
    /// </summary>
    public void SetMasterVolume(float value)
    {
        // ค่า Slider (0 ถึง 1) ถูกแปลงเป็น Logarithmic Scale สำหรับ Volume Mixer (-80dB ถึง 0dB)
        // Unity แนะนำให้ใช้ Mathf.Log10(value) * 20f
        // ถ้า value เป็น 0, Mathf.Log10(0) จะเกิด Error, จึงต้องมีเงื่อนไขนี้
        float volume = value > 0f ? Mathf.Log10(value) * 20f : -80f;

        mainMixer.SetFloat(MASTER_VOLUME_PARAM, volume);
        PlayerPrefs.SetFloat(MASTER_VOLUME_PARAM, value); // บันทึกค่าลง PlayerPrefs
        Debug.Log($"Master Volume set to: {value} (UI) / {volume} (Mixer)");
    }

    /// <summary>
    /// โหลดค่า Master Volume ที่บันทึกไว้และตั้งค่า Slider
    /// </summary>
    private void LoadVolume()
    {
        // โหลดค่าที่บันทึกไว้, ถ้าไม่มีให้ใช้ค่าเริ่มต้น 0.75f (ประมาณ 75%)
        float savedVolume = PlayerPrefs.GetFloat(MASTER_VOLUME_PARAM, 1f);
        masterVolumeSlider.value = savedVolume; // ตั้งค่า Slider ให้ตรงกับค่าที่โหลดมา
        SetMasterVolume(savedVolume); // ตั้งค่า Volume ให้ Mixer ด้วยค่าที่โหลดมา
        Debug.Log($"Master Volume loaded: {savedVolume}");
    }

    // เมื่อ Script ถูกปิดหรือ GameObject ถูกทำลาย ให้ลบ Listener ออก
    void OnDisable()
    {
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.onValueChanged.RemoveListener(SetMasterVolume);
        }
    }
}