using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CollisionDetectionSAT : MonoBehaviour
{
    public GameObject shapeA;
    public GameObject shapeB;

    private Color gizmoColor = Color.yellow;

    private bool isColliding;
    public float penetrationDepth;
    private Vector2 penetrationDir;
    private Vector2 mtv; // Minimum Translation Vector = penetrationDepth × penetrationDir

    public bool showGizmos = false;


    void Update()
    {
        if (shapeA == null || shapeB == null)
            return;

        Vector2[] vertShapeA = GetWorldVertices(shapeA);
        Vector2[] vertShapeB = GetWorldVertices(shapeB);

        isColliding = AreShapesColliding(vertShapeA, vertShapeB);
    }

    Vector2[] GetWorldVertices(GameObject obj)
    {
        PolygonCollider2D poly = obj.GetComponent<PolygonCollider2D>();
        if (poly != null)
        {
            Vector2[] worldPoints = new Vector2[poly.points.Length];
            for (int i = 0; i < poly.points.Length; i++)
            {
                Vector2 localPoint = poly.points[i];
                Vector3 worldPoint3D = obj.transform.TransformPoint((Vector3)localPoint);
                worldPoints[i] = (Vector2)worldPoint3D;
            }

            return worldPoints;
        }

        CircleCollider2D circle = obj.GetComponent<CircleCollider2D>();
        if (circle != null)
        {
            int segments = 20;
            float radius = circle.radius * obj.transform.lossyScale.x;
            Vector2 center = obj.transform.TransformPoint(circle.offset);
            Vector2[] points = new Vector2[segments];
            for (int i = 0; i < segments; i++)
            {
                float angle = 2 * Mathf.PI * i / segments;
                Vector2 local = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
                points[i] = center + local;
            }

            return points;
        }

        BoxCollider2D box = obj.GetComponent<BoxCollider2D>();
        if (box != null)
        {
            Vector2 size = box.size;
            Vector2 offset = box.offset;

            Vector2[] local = new Vector2[]
            {
                offset + new Vector2(-size.x, -size.y) * 0.5f,
                offset + new Vector2( size.x, -size.y) * 0.5f,
                offset + new Vector2( size.x,  size.y) * 0.5f,
                offset + new Vector2(-size.x,  size.y) * 0.5f,
            };

            Vector2[] worldPoints = new Vector2[local.Length];
            for (int i = 0; i < local.Length; i++)
            {
                Vector3 localPoint3D = new Vector3(local[i].x, local[i].y, 0f);
                Vector3 worldPoint3D = obj.transform.TransformPoint(localPoint3D);
                worldPoints[i] = (Vector2)worldPoint3D;
            }

            return worldPoints;
        }

        Debug.LogWarning("No PolygonCollider2D, CircleCollider2D or BoxCollider2D found.");

        return new Vector2[0];
    }


    bool AreShapesColliding(Vector2[] vertShapeA, Vector2[] vertShapeB)
    {
        float minOverlap = float.PositiveInfinity;
        Vector2 smallestAxis = Vector2.zero;

        foreach (var axis in GetAxes(vertShapeA).Concat(GetAxes(vertShapeB)))
        {
            ProjectPolygon(axis, vertShapeA, out float minA, out float maxA);
            ProjectPolygon(axis, vertShapeB, out float minB, out float maxB);

            if ((maxA < minB) || (maxB < minA))
            {
                // No overlap on this axis → separating axis found
                penetrationDepth = 0f;
                mtv = Vector2.zero;
                return false;
            }

            float overlap = Mathf.Min(maxA, maxB) - Mathf.Max(minA, minB);

            if (overlap < minOverlap)
            {
                minOverlap = overlap;
                smallestAxis = axis;
            }
        }

        penetrationDepth = minOverlap;
        penetrationDir = smallestAxis.normalized;
        mtv = penetrationDepth * penetrationDir;

        return true;
    }


    List<Vector2> GetAxes(Vector2[] vertShape)
    {
        List<Vector2> axes = new List<Vector2>();
        for (int i = 0; i < vertShape.Length; i++)
        {
            Vector2 edge = vertShape[(i + 1) % vertShape.Length] - vertShape[i];
            Vector2 normal = new Vector2(-edge.y, edge.x).normalized;
            axes.Add(normal);
        }

        return axes;
    }

    void ProjectPolygon(Vector2 axis, Vector2[] polygon, out float min, out float max)
    {
        float dot = Vector2.Dot(axis, polygon[0]);
        min = dot;
        max = dot;

        for (int i = 1; i < polygon.Length; i++)
        {
            dot = Vector2.Dot(axis, polygon[i]);

            if (dot < min)
            {
                min = dot;
            }
            else if (dot > max)
            {
                max = dot;
            }
        }
    }


    void OnDrawGizmos()
    {
        if (shapeA == null || shapeB == null)
            return;

        Gizmos.color = isColliding ? Color.red : gizmoColor;

        Vector2[] vertShapeA = GetWorldVertices(shapeA);
        Vector2[] vertShapeB = GetWorldVertices(shapeB);

        for (int i = 0; i < vertShapeA.Length; i++)
        {
            Vector2 a = vertShapeA[i];
            Vector2 b = vertShapeA[(i + 1) % vertShapeA.Length];
            Gizmos.DrawLine(a, b);
        }

        for (int i = 0; i < vertShapeB.Length; i++)
        {
            Vector2 a = vertShapeB[i];
            Vector2 b = vertShapeB[(i + 1) % vertShapeB.Length];
            Gizmos.DrawLine(a, b);
        }

        if (showGizmos)
        {
            Vector2 center = (shapeA.transform.position + shapeB.transform.position) * 0.5f;
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(center, center + mtv);
            Gizmos.DrawSphere(center + mtv, 0.08f);
        }
    }
}
