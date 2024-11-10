using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class vertex {
    public Vector3 position;
    public float weight;

    public vertex(Vector3 pos, float weight){
        this.position = pos;
        this.weight = weight;
    }
}

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class CustomRBCube : MonoBehaviour
{
    public int[,] Ibody = new int[,]
    {
        {0, 0, 0},
        {0, 0, 0},
        {0, 0, 0}
    };

    public List<vertex> verts = new List<vertex>();
    public Vector3[] vertices = new Vector3[]
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

    public int[] weights = new int[]
    {
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
    };
    public Mesh mesh;

    void CalculateInertiaTensor()
    {
        float mass = 0f;
        foreach (vertex v in verts)
        {
            mass += v.weight;
        }

        float halfLengthX = Mathf.Abs(vertices[1].x - vertices[0].x) / 2f;
        float halfLengthY = Mathf.Abs(vertices[3].y - vertices[0].y) / 2f;
        float halfLengthZ = Mathf.Abs(vertices[4].z - vertices[0].z) / 2f;

        float Ixx = (1f / 12f) * mass * (halfLengthY * halfLengthY + halfLengthZ * halfLengthZ);
        float Iyy = (1f / 12f) * mass * (halfLengthX * halfLengthX + halfLengthZ * halfLengthZ);
        float Izz = (1f / 12f) * mass * (halfLengthX * halfLengthX + halfLengthY * halfLengthY);

        Ibody[0, 0] = (int)Ixx;
        Ibody[1, 1] = (int)Iyy;
        Ibody[2, 2] = (int)Izz;
    }

    void Awake()
    {

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        
        int[] triangles = new int[]
        {
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

        CalculateInertiaTensor();

        mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
        for (int i = 0; i < vertices.Length; i++)
        {
            verts.Add(new vertex(vertices[i], weights[i]));
        }
    }

    public void ApplyTransformation(Matrix4x4 matrix)
    {
        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = matrix.MultiplyPoint3x4(vertices[i]);
        }
        mesh.vertices = vertices;
        mesh.RecalculateBounds();
    }
    
    void Update()
    {
        verts.Clear();
        for (int i = 0; i < vertices.Length; i++)
        {
            verts.Add(new vertex(vertices[i], weights[i]));
        }
        
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}