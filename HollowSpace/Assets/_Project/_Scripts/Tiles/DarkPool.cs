using UnityEngine;

public class DarkPool : MonoBehaviour {
    [SerializeField] private float damage = 1;
    [SerializeField] private float slowDownFactor = 0.5f;
    [SerializeField] private float intensityFactor = 0.03f;
    [SerializeField] private float dimTime = 1f;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            HandlePlayerEnter(other);
        }
        
        if (other.TryGetComponent<LightDimmingController>(out var ldc)) {
            ldc.DimLightsAsync(intensityFactor, dimTime);
        }
    }

    private void OnTriggerStay2D(Collider2D other) {
        // TODO: Decide if everything or just the player should take damage
        
        if (!other.TryGetComponent<Health>(out var h)) return;
        
        h.TakeDamage(damage * Time.fixedDeltaTime);
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) HandlePlayerExit(other);
        
        if (other.TryGetComponent<LightDimmingController>(out var ldc)) {
            ldc.ResetLightsAsync(dimTime);
        }
    }

    private void HandlePlayerEnter(Collider2D col) {
        // Slow down player
        PlayerController playerController = col.GetComponent<PlayerController>();
        if (playerController) {
            playerController.AdjustSpeed(slowDownFactor);
        }
    }
    private void HandlePlayerExit(Collider2D col) {
        // Reset player speed
        PlayerController playerController = col.GetComponent<PlayerController>();
        if (playerController) {
            playerController.ResetSpeed();
        }
    }
}