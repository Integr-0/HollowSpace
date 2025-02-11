using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [SerializeField, Tooltip("What to do when interacted with")] 
    private UnityEvent<InteractionAgent> _action;
    public void Interact(InteractionAgent agent)
    {
        print($"{agent.name} interacted with {name}");
        _action?.Invoke(agent);
    }
}
