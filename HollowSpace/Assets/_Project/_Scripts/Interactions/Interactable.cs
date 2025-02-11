using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [SerializeField, Tooltip("What to do when interacted with")] 
    private UnityEvent _action;
    public void Interact()
    {
        print("Interacted with " + name);
        _action?.Invoke();
    }
}
