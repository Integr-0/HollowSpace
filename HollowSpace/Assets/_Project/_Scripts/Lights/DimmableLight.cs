using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class DimmableLight : MonoBehaviour {
    public float Intensity { get; private set; }
    private Light2D _light2D;
    
    private void Awake() {
        _light2D = GetComponent<Light2D>();
        Intensity = _light2D.intensity;
    }
    
    public void Dim(float factor) {
        _light2D.intensity *= factor;
    }
    public void DimAsync(float factor, float duration) {
        if (duration <= 0) {
            Dim(factor);
            return;
        }
        
        StartCoroutine(DimRoutine(_light2D.intensity * factor, duration));
    }
    public void Reset() {
        _light2D.intensity = Intensity;
    }
    public void ResetAsync(float duration) {
        if (duration <= 0) {
            Reset();
            return;
        }
        
        StartCoroutine(DimRoutine(Intensity, duration));
    }
    public void SetIntensity(float intensity) {
        _light2D.intensity = intensity;
    }
    
    private IEnumerator DimRoutine(float intensity, float duration) {
        float startIntensity = _light2D.intensity;
        float t = 0;
        while (t < duration) {
            t += Time.deltaTime;
            _light2D.intensity = Mathf.Lerp(startIntensity, intensity, t / duration);
            yield return null;
        }
    }
}