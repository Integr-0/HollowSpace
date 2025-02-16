using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Object = UnityEngine.Object;

public static class LightChecker {
    private static Light2D[] _lights = Array.Empty<Light2D>();
    private static bool _initialized = false;

    private static List<Light2D> _pathfindLights = new();
    
    /// <summary>
    /// Checks, if a point is illuminated by any light in the scene.
    /// IMPORTANT: Only works for global and point 2D lights.
    /// This version doesn't use any raycasting, so it's faster, but less accurate.
    /// </summary>
    /// <param name="point">The point to check</param>
    /// /// <param name="includeOuterRadius">If the outer radius should be included in the check, or just the inner radius (for point lights)</param>
    /// <returns>If the point is illuminated by any light</returns>
    public static bool IsIlluminatedNoCast(Vector2 point, bool includeOuterRadius = true)
    {
        _lights = Object.FindObjectsOfType<Light2D>();
        _initialized = true;
        return _lights.Any(light => IsIlluminatedByLight(point, light, includeOuterRadius));
    }
    
    /// <summary>
    /// Checks, if a point is illuminated by any light in the scene.
    /// IMPORTANT: Only works for global and point 2D lights.
    /// This version uses a cached array of lights to avoid calling FindObjectsOfType every time.
    /// This version also doesn't use any raycasting, so it's faster, but less accurate.
    /// </summary>
    /// <param name="point">The point to check</param>
    /// /// <param name="includeOuterRadius">If the outer radius should be included in the check, or just the inner radius (for point lights)</param>
    /// <returns>If the point is illuminated by any light</returns>
    public static bool IsIlluminatedCachedNoCast(Vector2 point, bool includeOuterRadius = true) {
        if (_lights.Length == 0 && !_initialized) return IsIlluminatedNoCast(point); // If the array is empty, we need to update it
        
        return _lights.Any(light => IsIlluminatedByLight(point, light, includeOuterRadius));
    }
    
    /// <summary>
    /// Checks, if a point is illuminated by a light visible to Pathfinding scripts.
    /// </summary>
    /// <param name="point">The point to check</param>
    /// <param name="includeOuterRadius">If the outer radius should be included in the check, or just the inner radius (for point lights)</param>
    /// <returns>If the point is illuminated by a PathfindVisibleLight</returns>
    public static bool IsIlluminatedPathfindPoint(Vector2 point, bool includeOuterRadius = true) {
        return _pathfindLights.Any(light => IsIlluminatedByLight(point, light, includeOuterRadius));
    }

    /// <summary>
    /// Checks, if a point is illuminated by a specific light.
    /// </summary>
    /// <param name="point">The point to check</param>
    /// <param name="light">The light to check if it illuminates the point</param>
    /// <param name="includeOuterRadius">If the outer radius should be included in the check, or just the inner radius (for point lights)</param>
    /// <returns>If the point is illuminated by this specific light</returns>
    /// <exception cref="ArgumentException">Thrown when the light is of any other type than LightType.Point or LightType.Global</exception>
    public static bool IsIlluminatedByLight(Vector2 point, Light2D light, bool includeOuterRadius = true) {
        if (!light.enabled) return false;
        
        return light.lightType switch {
            Light2D.LightType.Point => IsIlluminatedByPointLight(point, light, includeOuterRadius),
            Light2D.LightType.Global => true,
            _ => throw new ArgumentException("Light type not supported.")
        };
    }

    private static bool IsIlluminatedByPointLight(Vector2 point, Light2D light, bool includeOuterRadius = true) {
        float distanceToLight = Vector2.Distance(point, light.transform.position);
        Vector2 directionToPoint = (point - (Vector2)light.transform.position).normalized;
        float angleToLight = Vector2.Angle(light.transform.up, directionToPoint);
        
        float lightRadius = includeOuterRadius ? light.pointLightOuterRadius : light.pointLightInnerRadius;
        float lightAngle = includeOuterRadius ? light.pointLightOuterAngle : light.pointLightInnerAngle;

        return distanceToLight <= lightRadius && angleToLight <= lightAngle * 0.5;
    }

    /// <summary>
    /// Adds a light to the list of lights visible to Pathfinding scripts.
    /// </summary>
    /// <param name="light">The light to add.</param>
    public static void AddPathfindVisibleLight(Light2D light) {
        _pathfindLights.Add(light);
    }
}