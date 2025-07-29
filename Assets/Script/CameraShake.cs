using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    private Vector3 _startPos;
    private float _shakeDuration;
    private float _shakeMagnitude;
    private float _shakeFrequency;
    private float _smoothness;

    private void Awake()
    {
        Instance = this;
    }

    public void Shake(float duration = 0.3f, float magnitude = 0.1f, float frequency = 20f, float smooth = 5f)
    {
        _startPos = transform.localPosition;
        _shakeDuration = duration;
        _shakeMagnitude = magnitude;
        _shakeFrequency = frequency;
        _smoothness = smooth;
    }

    private void Update()
    {
        if (_shakeDuration > 0f)
        {
            Vector3 shakeOffset = Vector3.zero;
            shakeOffset.x += Mathf.Lerp(0, Mathf.Cos(Time.time * _shakeFrequency) * _shakeMagnitude, _smoothness * Time.deltaTime);
            shakeOffset.y += Mathf.Lerp(0, Mathf.Sin(Time.time * _shakeFrequency * 1.2f) * _shakeMagnitude, _smoothness * Time.deltaTime);

            transform.localPosition = _startPos + shakeOffset;

            _shakeDuration -= Time.deltaTime;
        }
        else
        {
            // คืนค่ากลับตำแหน่งเดิม
            transform.localPosition = Vector3.Lerp(transform.localPosition, _startPos, _smoothness * Time.deltaTime);
        }
    }
}
