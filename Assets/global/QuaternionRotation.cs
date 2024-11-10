using UnityEngine;

public class QuaternionRotation : MonoBehaviour
{
    public float rotationSpeed = 20f; // Vitesse de rotation
    private Vector3[] originalVertices; // Sauvegarder les vertices originaux du cube
    private Vector3[] currentVertices;  // Sauvegarder les vertices transform�s
    private Mesh mesh;                  // R�f�rence au mesh du cube
    private bool isRotating = false;    // Pour savoir si on doit commencer � tourner

    void Start()
    {
        // R�cup�rer le mesh du cube
        mesh = GetComponent<MeshFilter>().mesh;

        // Sauvegarder les vertices d'origine et pr�parer les vertices pour la rotation
        originalVertices = mesh.vertices;
        currentVertices = new Vector3[originalVertices.Length];
        originalVertices.CopyTo(currentVertices, 0);
    }

    void Update()
    {
        // V�rifier si la touche espace est press�e pour commencer/arr�ter la rotation
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isRotating = !isRotating;
        }

        // Si la rotation est activ�e
        if (isRotating)
        {
            // Calculer l'angle de rotation (en radians)
            float angle = rotationSpeed * Time.deltaTime * Mathf.Deg2Rad;
            Vector3 rotationAxis = new Vector3(0, 1, 0); // Axe de rotation Y

            // Cr�er le quaternion � partir de l'angle et de l'axe
            Quaternion q = CreateQuaternion(rotationAxis, angle);

            // Cr�er la matrice de rotation � partir du quaternion
            Matrix4x4 rotationMatrix = CreateRotationMatrixFromQuaternion(q);

            // Appliquer la matrice de rotation � chaque vertex
            for (int i = 0; i < originalVertices.Length; i++)
            {
                currentVertices[i] = MultiplyPoint3x4(rotationMatrix, currentVertices[i]);
            }

            // Mettre � jour les vertices du mesh
            mesh.vertices = currentVertices;

            // Recalculer les normales et les bounds pour actualiser l'affichage du cube
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
        }
    }

    // Fonction pour cr�er un quaternion � partir d'un axe et d'un angle
    Quaternion CreateQuaternion(Vector3 axis, float angle)
    {
        // Normaliser l'axe de rotation
        axis.Normalize();

        float halfAngle = angle / 2;
        float sinHalfAngle = Mathf.Sin(halfAngle);

        return new Quaternion(axis.x * sinHalfAngle, axis.y * sinHalfAngle, axis.z * sinHalfAngle, Mathf.Cos(halfAngle));
    }

    // Fonction pour cr�er une matrice de rotation � partir d'un quaternion
    Matrix4x4 CreateRotationMatrixFromQuaternion(Quaternion q)
    {
        Matrix4x4 matrix = new Matrix4x4();

        // Calculer les �l�ments de la matrice de rotation
        float xx = q.x * q.x;
        float xy = q.x * q.y;
        float xz = q.x * q.z;
        float xw = q.x * q.w;
        float yy = q.y * q.y;
        float yz = q.y * q.z;
        float yw = q.y * q.w;
        float zz = q.z * q.z;
        float zw = q.z * q.w;

        matrix.m00 = 1 - 2 * (yy + zz);
        matrix.m01 = 2 * (xy - zw);
        matrix.m02 = 2 * (xz + yw);
        matrix.m03 = 0;

        matrix.m10 = 2 * (xy + zw);
        matrix.m11 = 1 - 2 * (xx + zz);
        matrix.m12 = 2 * (yz - xw);
        matrix.m13 = 0;

        matrix.m20 = 2 * (xz - yw);
        matrix.m21 = 2 * (yz + xw);
        matrix.m22 = 1 - 2 * (xx + yy);
        matrix.m23 = 0;

        matrix.m30 = 0;
        matrix.m31 = 0;
        matrix.m32 = 0;
        matrix.m33 = 1;

        return matrix;
    }

    // Fonction pour multiplier une position par une matrice 4x4
    Vector3 MultiplyPoint3x4(Matrix4x4 matrix, Vector3 point)
    {
        Vector3 result;
        result.x = matrix.m00 * point.x + matrix.m01 * point.y + matrix.m02 * point.z + matrix.m03;
        result.y = matrix.m10 * point.x + matrix.m11 * point.y + matrix.m12 * point.z + matrix.m13;
        result.z = matrix.m20 * point.x + matrix.m21 * point.y + matrix.m22 * point.z + matrix.m23;
        return result;
    }
}
