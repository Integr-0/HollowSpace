using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[RequireComponent(typeof(Collider2D))]
public class Interactable : MonoBehaviour
{
    [Tooltip("What to do when interacted with")] 
    public UnityEvent<InteractionAgent> Action;
    public void Interact(InteractionAgent agent)
    {
        print($"{agent.name} interacted with {name}");
        Action?.Invoke(agent);
    }
}
