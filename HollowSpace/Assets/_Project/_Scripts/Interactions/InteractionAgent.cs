using System.Linq;
using UnityEngine;

public class InteractionAgent : MonoBehaviour
{
    [Header("Interaction")] [SerializeField]
    public float interactionRange = 2f;

    [SerializeField] private LayerMask interactionLayer;
    [SerializeField] private KeyCode interactionKey = KeyCode.E;

    [Space, SerializeField] private Transform interactionIcon;

    private Interactable _closest;

    private void Update()
    {
        _closest = GetClosestInteractable();
        if (_closest) ShowIcon();
        else HideIcon();

        if (Input.GetKeyDown(interactionKey))
        {
            _closest?.Interact();
        }
    }

    private void ShowIcon()
    {
        interactionIcon.gameObject.SetActive(true);
        interactionIcon.position = _closest.transform.position + Vector3.up;
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