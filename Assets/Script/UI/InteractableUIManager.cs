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
    public GameObject CreateOtherInteractUI(Transform interactPoint)
    {
        if (otherInteractUI == null) return null;

        GameObject uiInstance = Instantiate(otherInteractUI, interactPoint);
        uiInstance.transform.localPosition = Vector3.zero;
        uiInstance.GetComponent<Canvas>().worldCamera = Camera.main;
        return uiInstance;
    }

}
public enum InteractableType
{
    Resource,
    Other
}