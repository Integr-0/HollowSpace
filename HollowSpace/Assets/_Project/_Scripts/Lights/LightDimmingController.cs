using UnityEngine;

public class LightDimmingController : MonoBehaviour {
    [SerializeField] private DimmableLight[] lights;
    
    public void DimLights(float factor) {
        foreach (DimmableLight light in lights) {
            light.Dim(factor);
        }
    }
    public void DimLightsAsync(float factor, float duration) {
        foreach (DimmableLight light in lights) {
            light.DimAsync(factor, duration);
        }
    }
    
    public void ResetLights() {
        foreach (DimmableLight light in lights) {
            light.Reset();
        }
    }
    public void ResetLightsAsync(float duration) {
        foreach (DimmableLight light in lights) {
            light.ResetAsync(duration);
        }
    }
}