using UnityEngine;

public class WorldBoundsGizmo : MonoBehaviour
{
    public Vector2 worldMin = new Vector2(-500, -500);
    public Vector2 worldMax = new Vector2(500, 500);
    public Color gizmoColor = Color.green;

    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Vector3 center = new Vector3(
            (worldMin.x + worldMax.x) / 2,
            transform.position.y,
            (worldMin.y + worldMax.y) / 2
        );

        Vector3 size = new Vector3(
            Mathf.Abs(worldMax.x - worldMin.x),
            0.1f,
            Mathf.Abs(worldMax.y - worldMin.y)
        );

        Gizmos.DrawWireCube(center, size);
    }
}
