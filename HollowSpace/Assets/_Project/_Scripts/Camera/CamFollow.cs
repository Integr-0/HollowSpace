using UnityEngine;

public class CamFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new(0, 0, -10);
    [SerializeField] private Vector2 deadzoneSize = new(2, 2);


    /*
        Current camera behavior:
        - If the target moves outside the deadzone, the camera instantly follows, but keeps the player on the edge of the deadzone
        - The camera does not move if the target is inside the deadzone

        This behaviour provides a good balance between keeping the player in the center of the screen and not moving the camera too much
        when the player moves slightly.
    */
    private void FixedUpdate() {
        if (target == null) return;

        Vector3 desiredPosition = CalculateDesiredPosition();
        if (ShouldFollow(desiredPosition))
        {
            transform.position = CalculateSmoothedPosition(desiredPosition);
        }
    }

    private Vector3 CalculateDesiredPosition()
    {
        return target.position + offset;
    }

    private Vector3 CalculateSmoothedPosition(Vector3 desiredPosition)
    {
        Vector3 smoothedPosition = transform.position;
        Vector3 delta = desiredPosition - transform.position;

        if (Mathf.Abs(delta.x) > deadzoneSize.x / 2)
        {
            smoothedPosition.x = desiredPosition.x - Mathf.Sign(delta.x) * deadzoneSize.x / 2;
        }

        if (Mathf.Abs(delta.y) > deadzoneSize.y / 2)
        {
            smoothedPosition.y = desiredPosition.y - Mathf.Sign(delta.y) * deadzoneSize.y / 2;
        }

        return smoothedPosition;
    }

    private bool ShouldFollow(Vector3 desiredPosition)
    {
        Vector3 delta = desiredPosition - transform.position;
        return Mathf.Abs(delta.x) > deadzoneSize.x / 2 || Mathf.Abs(delta.y) > deadzoneSize.y / 2;
    }

    private void OnDrawGizmos()
    {
        if (target == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, deadzoneSize);
    }
}