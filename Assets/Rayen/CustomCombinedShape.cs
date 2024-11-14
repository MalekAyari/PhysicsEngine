using UnityEngine;

public class CustomCombinedShape : MonoBehaviour
{
    public Mesh mesh;

    void Awake()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>() ?? gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>() ?? gameObject.AddComponent<MeshRenderer>();

        mesh = new Mesh();
        CombineMeshes(mesh);

        meshFilter.mesh = mesh;
        meshRenderer.material = new Material(Shader.Find("Standard"));
    }

    private void CombineMeshes(Mesh combinedMesh)
    {
        // Define cube vertices (stick part)
        Vector3[] cubeVertices = new Vector3[]
        {
            new Vector3(-0.2f, -1, -0.2f),
            new Vector3( 0.2f, -1, -0.2f),
            new Vector3( 0.2f,  0, -0.2f),
            new Vector3(-0.2f,  0, -0.2f),
            new Vector3(-0.2f, -1,  0.2f),
            new Vector3( 0.2f, -1,  0.2f),
            new Vector3( 0.2f,  0,  0.2f),
            new Vector3(-0.2f,  0,  0.2f)
        };

        int[] cubeTriangles = new int[]
        {
            0, 2, 1, 0, 3, 2,  4, 5, 6, 4, 6, 7,
            0, 7, 3, 0, 4, 7,  1, 2, 6, 1, 6, 5,
            0, 1, 5, 0, 5, 4,  3, 7, 6, 3, 6, 2
        };

        Vector3[] sphereVertices;
        int[] sphereTriangles;
        GenerateSphere(0.55f, out sphereVertices, out sphereTriangles);

        for (int i = 0; i < sphereVertices.Length; i++)
        {
            sphereVertices[i] += new Vector3(0, 0.5f, 0);
        }

        Vector3[] vertices = new Vector3[cubeVertices.Length + sphereVertices.Length];
        int[] triangles = new int[cubeTriangles.Length + sphereTriangles.Length];

        cubeVertices.CopyTo(vertices, 0);
        cubeTriangles.CopyTo(triangles, 0);

        for (int i = 0; i < sphereVertices.Length; i++)
        {
            vertices[cubeVertices.Length + i] = sphereVertices[i];
        }

        for (int i = 0; i < sphereTriangles.Length; i++)
        {
            triangles[cubeTriangles.Length + i] = sphereTriangles[i] + cubeVertices.Length;
        }

        Matrix4x4 rotationMatrix = CreateRotationMatrixZ(2 * Mathf.PI);

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = MultiplyPoint3x4(rotationMatrix, vertices[i]);
        }

        combinedMesh.vertices = vertices;
        combinedMesh.triangles = triangles;
        combinedMesh.RecalculateNormals();
    }

    private void GenerateSphere(float radius, out Vector3[] vertices, out int[] triangles)
    {
        int latSegments = 10;
        int lonSegments = 10;

        vertices = new Vector3[(latSegments + 1) * (lonSegments + 1)];
        triangles = new int[latSegments * lonSegments * 6];

        int vertIndex = 0;
        int triIndex = 0;

        for (int lat = 0; lat <= latSegments; lat++)
        {
            float theta = Mathf.PI * lat / latSegments;
            float sinTheta = Mathf.Sin(theta);
            float cosTheta = Mathf.Cos(theta);

            for (int lon = 0; lon <= lonSegments; lon++)
            {
                float phi = 2 * Mathf.PI * lon / lonSegments;
                float sinPhi = Mathf.Sin(phi);
                float cosPhi = Mathf.Cos(phi);

                float x = radius * sinTheta * cosPhi;
                float y = radius * cosTheta;
                float z = radius * sinTheta * sinPhi;
                vertices[vertIndex++] = new Vector3(x, y, z);

                if (lat < latSegments && lon < lonSegments)
                {
                    int current = lat * (lonSegments + 1) + lon;
                    int next = current + lonSegments + 1;

                    triangles[triIndex++] = current;
                    triangles[triIndex++] = next + 1;
                    triangles[triIndex++] = current + 1;

                    triangles[triIndex++] = current;
                    triangles[triIndex++] = next;
                    triangles[triIndex++] = next + 1;
                }
            }
        }
    }

    public Matrix4x4 CreateRotationMatrixZ(float angle)
    {
        Matrix4x4 matrix = new Matrix4x4();

        matrix.m00 = Mathf.Cos(angle); // cos(θ)
        matrix.m01 = -Mathf.Sin(angle); // -sin(θ)
        matrix.m02 = 0;
        matrix.m03 = 0;

        matrix.m10 = Mathf.Sin(angle); // sin(θ)
        matrix.m11 = Mathf.Cos(angle);  // cos(θ)
        matrix.m12 = 0;
        matrix.m13 = 0;

        matrix.m20 = 0;
        matrix.m21 = 0;
        matrix.m22 = 1;
        matrix.m23 = 0;

        matrix.m30 = 0;
        matrix.m31 = 0;
        matrix.m32 = 0;
        matrix.m33 = 1;

        return matrix;
    }

    Vector3 MultiplyPoint3x4(Matrix4x4 matrix, Vector3 point)
    {
        Vector3 result;
        result.x = matrix.m00 * point.x + matrix.m01 * point.y + matrix.m02 * point.z + matrix.m03;
        result.y = matrix.m10 * point.x + matrix.m11 * point.y + matrix.m12 * point.z + matrix.m13;
        result.z = matrix.m20 * point.x + matrix.m21 * point.y + matrix.m22 * point.z + matrix.m23;
        return result;
    }
}
