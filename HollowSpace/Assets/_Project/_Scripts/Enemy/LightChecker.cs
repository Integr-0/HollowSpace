using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Object = UnityEngine.Object;

public static class LightChecker {
    private static Light2D[] _lights = Array.Empty<Light2D>();
    private static bool _initialized = false;
    
    /// <summary>
    /// Checks, if a point is illuminated by any light in the scene.
    /// IMPORTANT: Only works for global and point 2D lights.
    /// This version doesn't use any raycasting, so it's faster, but less accurate.
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public static bool IsIlluminatedNoCast(Vector2 point)
    {
        _lights = Object.FindObjectsOfType<Light2D>();
        _initialized = true;
        return _lights.Any(light => IsIlluminatedByLight(point, light));
    }
    
    /// <summary>
    /// Checks, if a point is illuminated by any light in the scene.
    /// IMPORTANT: Only works for global and point 2D lights.
    /// This version uses a cached array of lights to avoid calling FindObjectsOfType every time.
    /// This version also doesn't use any raycasting, so it's faster, but less accurate.
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public static bool IsIlluminatedCachedNoCast(Vector2 point) {
        if (_lights.Length == 0 && !_initialized) return IsIlluminatedNoCast(point); // If the array is empty, we need to update it
        
        return _lights.Any(light => IsIlluminatedByLight(point, light));
    }

    public static bool IsIlluminatedByLight(Vector2 point, Light2D light) {
        return light.lightType switch {
            Light2D.LightType.Point => IsIlluminatedByPointLight(point, light),
            Light2D.LightType.Global => true,
            _ => throw new ArgumentException("Light type not supported.")
        };
    }

    private static bool IsIlluminatedByPointLight(Vector2 point, Light2D light) {
        float distanceToLight = Vector2.Distance(point, light.transform.position);
        Vector2 directionToPoint = (point - (Vector2)light.transform.position).normalized;
        float angleToLight = Vector2.Angle(light.transform.up, directionToPoint);

        return distanceToLight <= light.pointLightOuterRadius && angleToLight <= light.pointLightOuterAngle;
    }
}