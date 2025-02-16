using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// A PathfindVisibleLight is a light visible to the EnemyPathfind script for light-avoidant pathfinding.
/// </summary>
[RequireComponent(typeof(Light2D))]
public class PathfindVisibleLight : MonoBehaviour {
    private void Awake() {
        LightChecker.AddPathfindVisibleLight(GetComponent<Light2D>());
    }
}