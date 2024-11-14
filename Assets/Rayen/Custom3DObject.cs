using UnityEngine;

public class Custom3DObject : MonoBehaviour
{
    public Mesh mesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    void Awake()
    {
        // Ensure MeshFilter and MeshRenderer components are attached
        meshFilter = GetComponent<MeshFilter>() ?? gameObject.AddComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>() ?? gameObject.AddComponent<MeshRenderer>();

        // Define vertices of the cube
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(-1, -1, -1), // back-bottom-left
            new Vector3( 1, -1, -1), // back-bottom-right
            new Vector3( 1,  1, -1), // back-top-right
            new Vector3(-1,  1, -1), // back-top-left
            new Vector3(-1, -1,  1), // front-bottom-left
            new Vector3( 1, -1,  1), // front-bottom-right
            new Vector3( 1,  1,  1), // front-top-right
            new Vector3(-1,  1,  1)  // front-top-left
        };

        // Define triangles for each face
        int[] triangles = new int[]
        {
            // Back face
            0, 2, 1, 0, 3, 2,
            // Front face
            4, 5, 6, 4, 6, 7,
            // Left face
            0, 7, 3, 0, 4, 7,
            // Right face
            1, 2, 6, 1, 6, 5,
            // Bottom face
            0, 1, 5, 0, 5, 4,
            // Top face
            3, 7, 6, 3, 6, 2
        };

        // Create the Mesh and assign vertices and triangles
        mesh = new Mesh
        {
            vertices = vertices,
            triangles = triangles
        };

        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;

        // Add material for visibility
        meshRenderer.material = new Material(Shader.Find("Standard"));
    }

}
