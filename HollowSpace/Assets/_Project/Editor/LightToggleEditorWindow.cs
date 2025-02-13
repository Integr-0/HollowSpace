using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightToggleEditorWindow : EditorWindow
{
    private GameObject _globalLight;
    
    [MenuItem("Tools/Light Toggle")]
    public static void OpenWindow()
    {
        GetWindow<LightToggleEditorWindow>().Show();
    }

    private void OnDestroy()
    {
        TurnOnLights();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Turn on Lights"))
        {
            TurnOnLights();
        }
        if (GUILayout.Button("Turn off Lights"))
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
        
        // Create a new global light, if it doesn't exist already
        if (!_globalLight)
        {
            CreateGlobalLight();
        }
    }

    private void TurnOnLights()
    {
        var allLights = FindObjectsOfType<Light2D>();
        foreach (var light in allLights)
        {
            light.enabled = true;
        }
        
        if (_globalLight)
        {
            DestroyImmediate(_globalLight);
        }
    }

    private void CreateGlobalLight()
    {
        _globalLight = new GameObject("Global Light");
        var lightComponent = _globalLight.AddComponent<Light2D>();
        lightComponent.lightType = Light2D.LightType.Global;
    }
}
