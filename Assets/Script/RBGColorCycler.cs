using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class RGBColorCycler : MonoBehaviour
{
    [Tooltip("ระยะเวลา (วินาที) ให้ไล่ครบ 1 รอบสี")]
    public float cycleDuration = 3f;

    private Image _image;

    void Awake()
    {
        _image = GetComponent<Image>();
    }

    void Update()
    {
        // คำนวณค่า hue (0→1) ไล่ครบใน cycleDuration วินาที
        float hue = Mathf.Repeat(Time.time / cycleDuration, 1f);

        // แปลง HSV → RGB (saturation=1,value=1) ได้สีสดจัดเต็ม
        Color c = Color.HSVToRGB(hue, 1f, 1f);

        // เก็บ alpha เดิมไว้ด้วย ถ้าต้องการ
        c.a = _image.color.a;

        _image.color = c;
    }
}
