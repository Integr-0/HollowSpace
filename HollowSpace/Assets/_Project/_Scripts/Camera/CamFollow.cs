using UnityEngine;

public class CamFollow : MonoBehaviour
{
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new(0, 0, -10);
    [SerializeField] private Vector2 deadzoneSize = new Vector2(100, 100);

    private Vector3 _currentTarget;
    private bool _following = false;

    private void LateUpdate()
    {
        Vector3 targetPosition = target.position + offset;
        Vector3 delta = targetPosition - transform.position;

        if ((Mathf.Abs(delta.x) > deadzoneSize.x / 2 || Mathf.Abs(delta.y) > deadzoneSize.y / 2)
            && !_following)
        {
            _currentTarget = new Vector3(
                Mathf.Abs(delta.x) > deadzoneSize.x / 2 ? targetPosition.x : transform.position.x,
                Mathf.Abs(delta.y) > deadzoneSize.y / 2 ? targetPosition.y : transform.position.y,
                targetPosition.z
            );
            _following = true;
        }

        if (_following)
        {
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, _currentTarget, smoothSpeed);
            transform.position = smoothedPosition;
        }
        if ((_currentTarget - transform.position).sqrMagnitude < 0.1f)
        {
            _following = false;
        }
    }

    private void OnDrawGizmos()
    {
        if (target == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(target.position + offset, new Vector3(deadzoneSize.x, deadzoneSize.y, 0));
    }
}