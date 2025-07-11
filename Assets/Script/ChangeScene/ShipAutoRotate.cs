using UnityEngine;
using System.Collections;

public class ShipAutoRotate : MonoBehaviour
{
    IEnumerator Start()
    {
        // รอจนกว่า Instance จะไม่เป็น null (หรือรอ 1-2 เฟรม)
        int waitCount = 0;
        while (ShipController.Instance == null && waitCount < 100)
        {
            yield return null;
            waitCount++;
        }

        if (ShipController.Instance != null)
        {
            var rb = ShipController.Instance.GetComponent<Rigidbody>();
            if (rb != null)
                rb.MoveRotation(rb.rotation * Quaternion.Euler(0, 180, 0));
            else
                ShipController.Instance.transform.rotation = ShipController.Instance.transform.rotation * Quaternion.Euler(0, 180, 0);
        }
        else
        {
            Debug.LogError("ShipAutoRotate: ShipController.Instance is still null after waiting.");
        }
    }
}
