using System;
using UnityEngine;

public class InteractableUIManager : Singleton<InteractableUIManager>
{
    public GameObject resourceInteractUI;
    public GameObject otherInteractUI;

    public GameObject CreateResourceInteractUI(Transform interactPoint)
    {
        if (resourceInteractUI == null) return null;

        GameObject uiInstance = Instantiate(resourceInteractUI, interactPoint);
        uiInstance.transform.localPosition = Vector3.zero;
        uiInstance.GetComponent<Canvas>().worldCamera = Camera.main;
        return uiInstance;
    }
    public OtherInteractUI CreateOtherInteractUI(Transform interactPoint)
    {
        if (otherInteractUI == null) return null;

        GameObject uiInstance = Instantiate(otherInteractUI, interactPoint.position, interactPoint.rotation);
        uiInstance.transform.SetParent(interactPoint, false);
        OtherInteractUI otherUI = uiInstance.GetComponent<OtherInteractUI>();
        return otherUI;
    }

}
public enum InteractableType
{
    Resource,
    Other
}