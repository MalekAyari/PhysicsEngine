using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class vertex {
    public Vector3 position = Vector3.zero;
    public Vector3 localPosition;
    public Vector3 worldCenterOfMass;
    public float weight;

    public vertex(Vector3 pos, float weight, Vector3 worldCenterOfMass){
        this.position = pos;
        this.weight = weight;
        this.worldCenterOfMass = worldCenterOfMass;
    }

    public void CalculateLocalPosition(){
        localPosition = position - worldCenterOfMass;
    }
}

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class CustomRBCube : MonoBehaviour
{
    public float[,] Ibody = new float[,]
    {
        {0, 0, 0},
        {0, 0, 0},
        {0, 0, 0}
    };
    public float[,] Ibodyinv;
    public Vector3 position = Vector3.zero;
    public Vector3 rotation = Vector3.zero;
    public Vector3 localCenterOfMass = Vector3.zero;
    public Vector3 worldCenterOfMass = Vector3.zero;
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

        Ibodyinv = MatrixUtility.Inverse(Ibody);

        mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter.mesh = mesh;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertex newVert = new vertex(vertices[i], weights[i], worldCenterOfMass);
            newVert.CalculateLocalPosition();
            verts.Add(newVert);
        }
        CalculateIbody();

    }

    void Update()
    {
        localCenterOfMass = CalculateLocalCenterOfMass();
        worldCenterOfMass = localCenterOfMass + position;

        verts.Clear();
        for (int i = 0; i < vertices.Length; i++)
        {
            vertex newVert = new vertex(vertices[i], weights[i], worldCenterOfMass);
            newVert.CalculateLocalPosition();
            verts.Add(newVert);
        }
        
        mesh.vertices = vertices;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    

    public Matrix4x4 CreateTranslationMatrix(Vector3 translation)
    {
        Matrix4x4 matrix = Matrix4x4.identity; 

        matrix.m03 = translation.x;
        matrix.m13 = translation.y;
        matrix.m23 = translation.z; 

        return matrix; 
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
    
    void CalculateIbody()
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

        Ibody[0, 0] = Ixx;
        Ibody[1, 1] = Iyy;
        Ibody[2, 2] = Izz;
    }

    Vector3 CalculateLocalCenterOfMass()
    {
        Vector3 centerOfMass = Vector3.zero;
        float totalMass = 0;

        foreach (vertex v in verts)
        {
            centerOfMass += v.position * v.weight;
            totalMass += v.weight;
        }

        centerOfMass /= totalMass;

        return centerOfMass;
    }
    
    
}