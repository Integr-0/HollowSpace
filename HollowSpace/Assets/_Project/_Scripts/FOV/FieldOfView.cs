using UnityEngine;

// I love you Code Monkey and thanks for this tutoral: https://www.youtube.com/watch?v=CSeUMTaNFYk
public class FieldOfView : MonoBehaviour
{
    [SerializeField] private Transform followObject;
    [Header("Settings")]
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float fov = 90f;
    [SerializeField] private int rayCount = 50;
    [SerializeField] private float viewDistance = 50f;


    private Mesh mesh;
    Vector3 origin = Vector3.zero;
    private float startingAngle = 0f;

    private void Start()
    {
        mesh = new();
        GetComponent<MeshFilter>().mesh = mesh;
    }
    private void LateUpdate()
    {
        UpdatePosition();
        UpdateAimDirection();

        float angle = startingAngle;
        float angleIncrease = fov / rayCount;

        Vector3[] vertices = new Vector3[rayCount + 2];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = origin;

        int vertexIndex = 1;
        int triangleIndex = 0;
        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 vertex;
            var raycastHit2D = Physics2D.Raycast(origin, GetVectorFromAngle(angle), viewDistance, layerMask);
            if (raycastHit2D.collider == null)
            {
                // No hit, draw full length
                vertex = origin + GetVectorFromAngle(angle) * viewDistance;
            }
            else
            {
                // Hit object, draw line to hit point
                vertex = raycastHit2D.point;
            }
            vertices[vertexIndex] = vertex;

            if (i > 0)
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;
            }

            vertexIndex++;

            angle -= angleIncrease;
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateBounds(); // Without this, the mesh will not be visible after a certain distance
    }

    private void UpdateAimDirection() {
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var aimDirection = (mousePos - followObject.position).normalized;
        SetAimDirection(aimDirection);
    }
    private void UpdatePosition() {
        SetOrigin(followObject.position);
    }


    public void SetOrigin(Vector3 origin)
    {
        this.origin = origin;
    }

    public void SetAimDirection(Vector3 aimDirection)
    {
        startingAngle = GetAngleFromVectorFloat(aimDirection) + fov / 2f;
    }

    private static Vector3 GetVectorFromAngle(float angle)
    {
        float angleRad = angle * (Mathf.PI / 180);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }
    private static float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }
}
