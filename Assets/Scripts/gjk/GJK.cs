using UnityEngine;
using System.Collections.Generic;

public class GJK
{
    private const int MaxIterations = 20;
    private List<Vector3> simplex = new List<Vector3>();
    private Vector3 direction;

    public bool Intersect(Vector3[] shapeA, Vector3[] shapeB)
    {
        simplex.Clear();

        direction = shapeB[0] - shapeA[0];
        direction = direction.normalized;

        if (direction == Vector3.zero)
            direction = Vector3.right;

        Vector3 initialSupportPoint = Support(shapeA, shapeB, direction); // first support point in the Minkowski difference
        simplex.Add(initialSupportPoint);

        direction = -simplex[0]; // setting the direction vector from the current simplex point toward the origin (0,0,0)
        direction = direction.normalized;

        for (int i = 0; i < MaxIterations; i++)
        {
            Vector3 newPoint = Support(shapeA, shapeB, direction);

            if (Vector3.Dot(newPoint, direction) < 0) // the new support point lies beyond 90°, behind the direction vector
                return false;

            simplex.Add(newPoint);

            if (HandleSimplex(simplex))
                return true;
        }

        return false;
    }

    private Vector3 Support(Vector3[] shapeA, Vector3[] shapeB, Vector3 direction)
    {
        Vector3 supportA = SupportFunction.ConvexHull(shapeA, direction);
        Vector3 supportB = SupportFunction.ConvexHull(shapeB, -direction);

        return supportA - supportB;
    }

    private bool HandleSimplex(List<Vector3> simplex)
    {
        if (simplex.Count == 2) return HandleLine(simplex);
        if (simplex.Count == 3) return HandleTriangle(simplex);
        if (simplex.Count == 4) return HandleTetrahedron(simplex);

        return false;
    }

    private bool HandleLine(List<Vector3> simplex)
    {
        // Simplex points (A is the newest point added)
        Vector3 A = simplex[1];
        Vector3 B = simplex[0];
        Vector3 AB = B - A;
        Vector3 AO = -A;

        direction = Vector3.Cross(Vector3.Cross(AB, AO), AB);
        direction = direction.normalized;

        return false;
    }


    private bool HandleTriangle(List<Vector3> simplex)
    {
        // Simplex points (A is the newest point added)
        Vector3 A = simplex[2];
        Vector3 B = simplex[1];
        Vector3 C = simplex[0];
        Vector3 AO = -A;

        // Compute the triangle normal
        Vector3 normal = Vector3.Cross(B - A, C - A).normalized;

        // Determine if the origin lies above, below, or on the triangle plane
        float side = Vector3.Dot(normal, AO.normalized);

        // side == 0 → coplanar
        // side > 0 → in front (same side as normal)
        // side < 0 → behind (opposite side)
        if (Mathf.Abs(side) > 0.001f)
        {
            // Flip normal if it's pointing away from the origin
            if (side < 0)
                normal = -normal;

            //Debug.Log("Non-coplanar: origin above/below plane");

            direction = normal; // update search direction perpendicular to the triangle plane
            return false;
        }

        // --- Coplanar case: check if origin lies inside the triangle ---

        // Compute total triangle area (ABC)
        float areaABC = Vector3.Cross(B - A, C - A).magnitude * 0.5f;

        // Compute sub-areas with the origin (O)
        float areaOBC = Vector3.Cross(B, C).magnitude * 0.5f; // opposite A
        float areaOCA = Vector3.Cross(C, A).magnitude * 0.5f; // opposite B
        float areaOAB = Vector3.Cross(A, B).magnitude * 0.5f; // opposite C

        // Compute barycentric coordinates
        float u = areaOBC / areaABC; // for vertex A
        float v = areaOCA / areaABC; // for vertex B
        float w = areaOAB / areaABC; // for vertex C

        // If origin lies inside triangle (all barycentrics ≥ 0 and sum ≈ 1)
        if (u >= 0 && v >= 0 && w >= 0 && Mathf.Abs(u + v + w - 1f) < 0.001f)
        {
            //Debug.Log("Inside triangle: u = " + u + ", v = " + v + ", w = " + w + ", sum = " + (u + v + w));
            return true; // Origin is enclosed
        }

        // Otherwise, figure out which edge region the origin lies in
        if (u < 0)
        {
            // Outside edge BC → remove A
            simplex.RemoveAt(2);
            direction = Vector3.Cross(Vector3.Cross(B - C, -C), B - C).normalized;
        }
        else if (v < 0)
        {
            // Outside edge AC → remove B
            simplex.RemoveAt(1);
            direction = Vector3.Cross(Vector3.Cross(C - A, AO), C - A).normalized;
        }
        else if (w < 0)
        {
            // Outside edge AB → remove C
            simplex.RemoveAt(0);
            direction = Vector3.Cross(Vector3.Cross(B - A, AO), B - A).normalized;
        }

        return false;
    }


    private bool HandleTetrahedron(List<Vector3> simplex)
    {
        //Debug.Log("Inside tetrahedron !!");

        // Simplex points (A is the newest point added)
        Vector3 A = simplex[3];
        Vector3 B = simplex[2];
        Vector3 C = simplex[1];
        Vector3 D = simplex[0];
        Vector3 AO = -A;

        // Compute face normals for the three faces that share vertex A
        Vector3 ABC = Vector3.Cross(B - A, C - A);
        Vector3 ACD = Vector3.Cross(C - A, D - A);
        Vector3 ADB = Vector3.Cross(D - A, B - A);

        // Check which side of each face the origin lies on
        // If the origin lies outside the ABC face
        if (Vector3.Dot(ABC, AO) > 0)
        {
            simplex.RemoveAt(0); // remove D (the point opposite to ABC)

            direction = ABC; // search direction is perpendicular to ABC toward the origin
            direction = direction.normalized;

            return false;
        }

        // If the origin lies outside the ACD face
        if (Vector3.Dot(ACD, AO) > 0)
        {
            simplex.RemoveAt(2); // remove B (the point opposite to ACD)

            direction = ACD; // search direction is perpendicular to ACD toward the origin
            direction = direction.normalized;

            return false;
        }

        // If the origin lies outside the ADB face
        if (Vector3.Dot(ADB, AO) > 0)
        {
            simplex.RemoveAt(1); // remove C (the point opposite to ADB)

            direction = ADB; // search direction is perpendicular to ADB toward the origin
            direction = direction.normalized;

            return false;
        }

        // If the origin is not outside any face,
        // it must be inside the tetrahedron → collision detected 
        return true;
    }
}
