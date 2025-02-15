using UnityEngine;

public class DoorState : MonoBehaviour {
    public bool IsOpen { get; private set; }
    private BoxCollider2D _collider;

    public void Start() {
        _collider = GetComponent<BoxCollider2D>();
    }

    public void ToggleOpen() {
        IsOpen = !IsOpen;
        _collider.isTrigger = IsOpen;
    }
}