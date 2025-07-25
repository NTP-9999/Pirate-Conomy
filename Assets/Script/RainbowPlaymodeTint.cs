using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

[InitializeOnLoad]
public static class RainbowPlaymodeTint
{
    private static float hue = 0f;
    private static PropertyInfo playmodeTintProp;

    static RainbowPlaymodeTint()
    {
        // Get internal 'PreferencesWindow' playmode tint field
        Type editorPrefsColorsType = typeof(Editor).Assembly.GetType("UnityEditor.EditorPrefsColors");
        if (editorPrefsColorsType != null)
        {
            playmodeTintProp = editorPrefsColorsType.GetProperty("playmodeTint", BindingFlags.Static | BindingFlags.NonPublic);
        }

        EditorApplication.update += OnEditorUpdate;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnEditorUpdate()
    {
        if (EditorApplication.isPlaying)
        {
            hue += 0.01f;
            if (hue > 1f) hue = 0f;

            Color rainbow = Color.HSVToRGB(hue, 0.8f, 1f);
            SetPlaymodeTint(rainbow);
        }
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            // Reset to default gray-ish
            SetPlaymodeTint(new Color(0.22f, 0.22f, 0.22f, 1f));
        }
    }

    private static void SetPlaymodeTint(Color color)
    {
        if (playmodeTintProp != null)
        {
            playmodeTintProp.SetValue(null, color, null);
            // repaint windows
            EditorApplication.RepaintHierarchyWindow();
            EditorApplication.RepaintProjectWindow();
            SceneView.RepaintAll();
        }
    }
}
