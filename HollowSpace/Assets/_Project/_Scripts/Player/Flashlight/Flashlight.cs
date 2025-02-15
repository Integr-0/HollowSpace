using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Flashlight : MonoBehaviour {
    [Header("Flashlight Settings")] 
    [SerializeField, Tooltip("The time (in seconds) it takes a single battery to run out")]
    private float batteryLife = 100f;

    [SerializeField] private int maxBatteries = 2;
    [SerializeField] private int startingBatteries = 1;

    [Header("Flicker")] 
    [SerializeField, Tooltip("At what battery life the light will start flickering")]
    private float flickerThreshold = 10f;

    [SerializeField, Tooltip("How fast the light will flicker")]
    private float flickerSpeed = 10f;

    [Header("References")] 
    [SerializeField]
    private new Light2D light;

    private float _currentBatteryLife;
    private int _currentBatteries;
    private float _defaultIntensity;

    private void Start() {
        _defaultIntensity = light.intensity;
        _currentBatteries = startingBatteries;
    }

    /// <summary>
    /// Inserts a battery into the flashlight.
    /// </summary>
    /// <returns>True if the battery was inserted, False if there were already the maximum amount of batteries inside</returns>
    public bool InsertBattery() {
        if (_currentBatteries >= maxBatteries) return false;
        
        _currentBatteries++;
        return true;
    }

    /// <summary>
    /// Uses a battery from the flashlight to restore the battery life.
    /// </summary>
    /// <returns>True if a battery was used, False if there were no more batteries left to use</returns>
    private bool UseBattery() {
        if (_currentBatteries <= 0) return false;

        _currentBatteries--;
        _currentBatteryLife = batteryLife;
        return true;
    }

    private void Update() {
        if (_currentBatteryLife > 0) {
            _currentBatteryLife -= Time.deltaTime;
        }
        else if (!UseBattery()) {
            // If there are no batteries left, turn off the light
            light.intensity = 0;
            return;
        }

        if (_currentBatteryLife <= flickerThreshold) {
            light.intensity = Mathf.PingPong(Time.time * flickerSpeed, _defaultIntensity);
        }
        else {
            light.intensity = _defaultIntensity;
        }
    }
}