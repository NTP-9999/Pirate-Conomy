using UnityEngine;
using System.Collections;

public class DeathCameraTransition : MonoBehaviour
{
    [Tooltip("จุดปลายทางที่กล้องจะเคลื่อนไป")]
    public Transform target;

    [Tooltip("ความยาวเวลาที่จะ transition (วินาที)")]
    public float duration = 2f;

    // เรียกเมื่อต้องการเริ่ม effect
    public void StartTransition()
    {
        StartCoroutine(TransitionCoroutine());
    }

    private IEnumerator TransitionCoroutine()
    {
        // เก็บจุดเริ่มต้นและมุมเริ่มต้นของกล้อง
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // Lerp ตำแหน่ง
            transform.position = Vector3.Lerp(startPos, target.position, t);
            // Slerp มุมกล้อง
            transform.rotation = Quaternion.Slerp(startRot, target.rotation, t);

            yield return null;
        }

        // ให้แน่ใจว่าถึงเป้าสุดท้ายเป๊ะ ๆ
        transform.position = target.position;
        transform.rotation = target.rotation;
    }
}
