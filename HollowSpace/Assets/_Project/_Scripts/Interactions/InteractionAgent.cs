using UnityEngine;

public class InteractionAgent : MonoBehaviour
{
    [Header("Interaction")] [SerializeField]
    public float interactionRange = 2f;

    [SerializeField] private LayerMask interactionLayer;
    [SerializeField] private KeyCode interactionKeyOverride = KeyCode.None;

    [Space, SerializeField] private Transform interactionIcon;
    
    private bool _hasOverride => interactionKeyOverride != KeyCode.None;

    private bool _interactionPressed =>
        _hasOverride ? Input.GetKeyDown(interactionKeyOverride) : Input.GetButtonDown("Interact");

    private Interactable _closest;

    private void Update()
    {
        _closest = GetClosestInteractable();
        if (_closest) ShowIcon();
        else HideIcon();

        if (_interactionPressed)
        {
            _closest?.Interact(this);
        }
    }

    private void ShowIcon()
    {
        interactionIcon.gameObject.SetActive(true);
        interactionIcon.position = Camera.main.WorldToScreenPoint(_closest.transform.position + Vector3.up);
    }

    private void HideIcon()
    {
        interactionIcon.gameObject.SetActive(false);
    }

    private Interactable GetClosestInteractable()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactionRange, interactionLayer);
        Interactable closestInteractable = null;
        float closestSqrDistance = float.MaxValue;

        foreach (var collider in colliders)
        {
            Interactable interactable = collider.GetComponent<Interactable>();
            if (interactable != null)
            {
                float sqrDistance = (transform.position - interactable.transform.position).sqrMagnitude;
                if (sqrDistance < closestSqrDistance)
                {
                    closestInteractable = interactable;
                    closestSqrDistance = sqrDistance;
                }
            }
        }

        return closestInteractable;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}