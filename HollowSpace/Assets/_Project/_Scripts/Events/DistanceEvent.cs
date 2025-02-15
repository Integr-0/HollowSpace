using UnityEngine;
using UnityEngine.Events;

public class DistanceEvent : MonoBehaviour {
    [SerializeField] private Transform target;
    [SerializeField] private float distance;

    public UnityEvent OnDistanceEnter;
    public UnityEvent OnDistanceExit;
    public UnityEvent OnDistanceStay;
    
    private bool isInside;
    
    private void Update() {
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        
        if (distanceToTarget <= distance) {
            if (!isInside) {
                isInside = true;
                OnDistanceEnter.Invoke();
            }
            else {
                OnDistanceStay.Invoke();
            }
        }
        else if (isInside) {
            isInside = false;
            OnDistanceExit.Invoke();
        }
    }
}