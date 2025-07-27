using UnityEngine;
using System.Collections;

public class SmallIsland : SceneChanger
{
    [Header("Small Island Settings")]
    [SerializeField] private string islandName = "Small Island";
    public AudioSource fadeout;

    protected override void Start()
    {
        targetSceneName = islandName;
        base.Start();
    }

    protected override void ChangeScene()
    {
        if (fadeout != null && fogEffect != null)
        {
            StartCoroutine(PlayFadeAndWaitForAudio());
        }
        else
        {
            base.ChangeScene(); // fallback
        }
    }

    private IEnumerator PlayFadeAndWaitForAudio()
    {
        // Start fade-in effect
        fadeout.Play();
        yield return StartCoroutine(fogEffect.FadeIn(fogFadeTime));

        // Play audio
        

        // Wait for audio to finish
        while (fadeout.isPlaying)
        {
            yield return null;
        }

        Debug.Log($"Traveling to Small Island: {islandName}...");
        base.ChangeScene();
    }
}
