using UnityEngine;
using TMPro; 
using System.Collections; 

public class LoadingDots : MonoBehaviour
{
    public TextMeshProUGUI targetText; 
    public float dotInterval = 0.5f; 

    private string baseText = "Loading";
    private int currentDotCount = 0;
    private Coroutine dotRoutine;

    private void OnEnable()
    {
        
        if (targetText == null)
        {
            targetText = GetComponent<TextMeshProUGUI>();
            if (targetText == null) 
            {
                Debug.LogError("LoadingDots: targetText is not assigned and TextMeshProUGUI component not found on this GameObject.", this);
                enabled = false; 
                return;
            }
        }

        if (dotRoutine != null)
        {
            StopCoroutine(dotRoutine);
        }
        dotRoutine = StartCoroutine(AnimateDotsRoutine());
    }

    private void OnDisable()
    {
        if (dotRoutine != null)
        {
            StopCoroutine(dotRoutine);
        }
        if (targetText != null)
        {
            targetText.text = baseText;
            targetText.enabled = true;
        }
    }

    IEnumerator AnimateDotsRoutine()
    {
        while (true) 
        {
            string displayText = baseText;
            for (int i = 0; i < currentDotCount; i++)
            {
                displayText += ".";
            }
            targetText.text = displayText; 

            currentDotCount++; 

            if (currentDotCount > 3)
            {
                currentDotCount = 0;
            }

            yield return new WaitForSeconds(dotInterval);
        }
    }
}