using UnityEngine;

public class GJKTester : MonoBehaviour
{
    public GameObject shapeA;
    public GameObject shapeB;

    private Vector3[] shapeAVertices;
    private Vector3[] shapeBVertices;
    private bool isColliding = false;

    void Update()
    {
        shapeAVertices = GetWorldSpaceVertices(shapeA);
        shapeBVertices = GetWorldSpaceVertices(shapeB);

        GJK solver = new GJK();
        isColliding = solver.Intersect(shapeAVertices, shapeBVertices);
    }

    private Vector3[] GetWorldSpaceVertices(GameObject obj)
    {
        Mesh mesh = obj.GetComponent<MeshFilter>().sharedMesh;
        Vector3[] localVertices = mesh.vertices;
        Vector3[] worldVertices = new Vector3[localVertices.Length];

        for (int i = 0; i < localVertices.Length; i++)
        {
            worldVertices[i] = obj.transform.TransformPoint(localVertices[i]);
        }

        return worldVertices;
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        if (!isColliding) return;

        Gizmos.color = Color.red;
        foreach (var v in shapeAVertices)
        {
            Gizmos.DrawSphere(v, 0.015f);
        }

        foreach (var v in shapeBVertices)
        {
            Gizmos.DrawSphere(v, 0.015f);
        }
    }
}
