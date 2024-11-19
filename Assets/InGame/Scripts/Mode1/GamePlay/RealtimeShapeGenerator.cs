using System.Collections.Generic;
using UnityEngine;

public class RealtimeShapeGenerator : MonoBehaviour
{
    [Header("Shape Properties")]
    [SerializeField] private float lineWidth = 0.05f; // Line width for the shape outline
    [SerializeField] private Material shapeMaterial;  // Material for the shape mesh
    [SerializeField] private float gravityScale = 1.0f; // Gravity setting for Rigidbody
    [SerializeField] private float mass = 1.0f; // Mass setting for Rigidbody

    private GameObject shapeObject;
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private EdgeCollider2D edgeCollider;
    private Rigidbody2D rb;
    private Mesh mesh;

    // Method to initialize and create the shape with mesh and physics
    public void GenerateShape(List<Vector2> points)
    {
        // Create a new GameObject to hold the shape
        shapeObject = new GameObject("GeneratedShape");

        // Generate mesh based on points and line width
        mesh = GenerateLineMeshFromPoints(points, lineWidth);

        // Add MeshFilter and MeshRenderer components for displaying the mesh
        meshFilter = shapeObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        meshRenderer = shapeObject.AddComponent<MeshRenderer>();
        meshRenderer.material = shapeMaterial; // Use Inspector-assigned material

        // Add EdgeCollider2D to match the drawn shape outline for physics collision
        edgeCollider = shapeObject.AddComponent<EdgeCollider2D>();
        edgeCollider.points = points.ToArray();

        // Add Rigidbody2D for physics and set properties based on Inspector values
        rb = shapeObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;
        rb.mass = mass;
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    // Generate a line-based mesh from points with adjustable line width
    private Mesh GenerateLineMeshFromPoints(List<Vector2> points, float width)
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[points.Count * 2];
        int[] triangles = new int[(points.Count - 1) * 6];

        for (int i = 0; i < points.Count; i++)
        {
            Vector2 point = points[i];
            Vector2 direction = Vector2.zero;

            // Determine direction of the line segment
            if (i < points.Count - 1)
                direction = (points[i + 1] - point).normalized;
            else
                direction = (point - points[i - 1]).normalized;

            Vector2 perpendicular = new Vector2(-direction.y, direction.x) * width * 0.5f;

            vertices[i * 2] = point + perpendicular;
            vertices[i * 2 + 1] = point - perpendicular;
        }

        int triangleIndex = 0;
        for (int i = 0; i < points.Count - 1; i++)
        {
            int startIndex = i * 2;

            triangles[triangleIndex++] = startIndex;
            triangles[triangleIndex++] = startIndex + 2;
            triangles[triangleIndex++] = startIndex + 1;

            triangles[triangleIndex++] = startIndex + 1;
            triangles[triangleIndex++] = startIndex + 2;
            triangles[triangleIndex++] = startIndex + 3;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }
}
