using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightToggleEditorWindow : EditorWindow
{
    [MenuItem("Tools/Light Toggle")]
    public static void OpenWindow()
    {
        GetWindow<LightToggleEditorWindow>("Light Toggle");
    }

    private void OnDestroy()
    {
        TurnOnLights();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Enable Lights"))
        {
            TurnOnLights();
        }
        if (GUILayout.Button("Disable Lights"))
        {
            TurnOffLights();
        }
    }
    
    private void TurnOffLights()
    {
        // Find all lights in the scene
        var allLights = FindObjectsOfType<Light2D>();
        foreach (var light in allLights)
        {
            light.enabled = false;
        }
    }

    private void TurnOnLights()
    {
        var allLights = FindObjectsOfType<Light2D>();
        foreach (var light in allLights)
        {
            light.enabled = true;
        }
    }
}
