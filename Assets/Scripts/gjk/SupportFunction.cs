using UnityEngine;

public static class SupportFunction
{
    public static Vector3 ConvexHull(Vector3[] worldVertices, Vector3 direction)
    {
        float maxDot = float.NegativeInfinity;
        Vector3 supportVert = Vector3.zero;

        foreach (var vertex in worldVertices)
        {
            float dot = Vector3.Dot(vertex, direction);
            if (dot > maxDot)
            {
                maxDot = dot;
                supportVert = vertex;
            }
        }

        return supportVert;
    }
}
