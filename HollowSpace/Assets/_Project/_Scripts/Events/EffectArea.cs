using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class EffectArea : MonoBehaviour
{
    [SerializeField] private UnityEvent<Collider2D> onEntered;
    [SerializeField] private UnityEvent onPlayerEntered;

    void Start()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        onEntered?.Invoke(other);
        
        if (other.CompareTag("Player"))
        {
            onPlayerEntered?.Invoke();
        }
    }

    public void PrintColliderInfo(Collider2D c)
    {
        Debug.Log($"{c.name} entered effect area {name}");
    }
}
