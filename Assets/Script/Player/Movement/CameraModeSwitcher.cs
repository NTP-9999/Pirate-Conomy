using UnityEngine;

public class CameraModeSwitcher : MonoBehaviour
{
    public GameObject fpsCamObject;           // กล้อง FPS (มีกล้อง + script FPS)
    public GameObject tpsCamObject;           // กล้อง TPS (มีกล้อง + script TPS)
    public Camera uiCamera;                   // กล้องที่ Render UI

    public Transform fpsCamPosition;          // ตำแหน่งกล้อง FPS
    public Transform tpsCamPosition;          // ตำแหน่งกล้อง TPS

    private bool isFirstPerson = true;

    void Start()
    {
        SetToFirstPerson();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V)) // ปุ่มสลับมุมมอง
        {
            if (isFirstPerson)
                SetToThirdPerson();
            else
                SetToFirstPerson();
        }
    }

    void SetToFirstPerson()
    {
        isFirstPerson = true;
        fpsCamObject.SetActive(true);
        tpsCamObject.SetActive(false);

        // ✅ เปิด FPS Script & ปิด TPS
        fpsCamObject.GetComponent<FirstPersonCamera>().enabled = true;
        tpsCamObject.GetComponent<ThirdPersonCamera>().enabled = false;

        // ✅ ย้ายกล้องหลักไปตำแหน่ง FPS
        fpsCamObject.transform.position = fpsCamPosition.position;
        fpsCamObject.transform.rotation = fpsCamPosition.rotation;

        // ✅ Sync UI กล้องด้วยถ้าต้องการ
        SyncUICamera(fpsCamObject.transform);
    }

    void SetToThirdPerson()
    {
        isFirstPerson = false;
        tpsCamObject.SetActive(true);
        fpsCamObject.SetActive(false);

        // ✅ เปิด TPS Script & ปิด FPS
        fpsCamObject.GetComponent<FirstPersonCamera>().enabled = false;
        tpsCamObject.GetComponent<ThirdPersonCamera>().enabled = true;

        // ✅ ย้ายกล้องหลักไปตำแหน่ง TPS
        tpsCamObject.transform.position = tpsCamPosition.position;
        tpsCamObject.transform.rotation = tpsCamPosition.rotation;

        // ✅ Sync UI กล้องด้วยถ้าต้องการ
        SyncUICamera(tpsCamObject.transform);
    }

    void SyncUICamera(Transform target)
    {
        if (uiCamera != null)
        {
            uiCamera.transform.position = target.position;
            uiCamera.transform.rotation = target.rotation;
        }
    }
}
