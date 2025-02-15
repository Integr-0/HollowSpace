using UnityEngine;

public class InteractionAgent : MonoBehaviour {
    [SerializeField] private float interactionRange = 2f;

    [SerializeField] private LayerMask interactionLayer;
    [SerializeField] private KeyCode interactionKeyOverride = KeyCode.None;

    [Space, SerializeField] private Transform interactionIcon;

    [SerializeField, Tooltip("The camera to use for transforming the position of the icon to screen-space (Leave empty to use main camera)")]
    private Camera cam;

    private bool HasOverride => interactionKeyOverride != KeyCode.None;

    private bool InteractionPressed =>
        HasOverride ? Input.GetKeyDown(interactionKeyOverride) : Input.GetButtonDown("Interact");
    
    private Camera Cam => cam ? cam : Camera.main;

    private Interactable _closest;

    private void Update() {
        _closest = GetClosestInteractable();
        if (_closest) ShowIcon();
        else HideIcon();

        if (InteractionPressed) {
            _closest?.Interact(this);
        }
    }

    private void ShowIcon() {
        interactionIcon.gameObject.SetActive(true);
        interactionIcon.position = Cam.WorldToScreenPoint(_closest.transform.position + new Vector3(0, 0.5f, 0));
    }

    private void HideIcon() {
        interactionIcon.gameObject.SetActive(false);
    }

    private Interactable GetClosestInteractable() {
        Collider2D[] colliders = GetInteractablesInRange(3);
        
        Interactable closestInteractable = null;
        float closestSqrDistance = float.MaxValue;

        foreach (Collider2D col in colliders) {
            if (!col.TryGetComponent<Interactable>(out var interactable)) continue;
            
            float sqrDistance = (transform.position - interactable.transform.position).sqrMagnitude;
            if (sqrDistance < closestSqrDistance) {
                closestInteractable = interactable;
                closestSqrDistance = sqrDistance;
            }
        }

        return closestInteractable;
    }
    
    private Collider2D[] GetInteractablesInRange(int allocSize) {
        Collider2D[] results = new Collider2D[allocSize];
        int size = Physics2D.OverlapCircleNonAlloc(transform.position, interactionRange, results, interactionLayer);
        return results[..size];
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}