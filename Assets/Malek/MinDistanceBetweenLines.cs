using UnityEngine;

public class MinDistanceBetweenLines : MonoBehaviour
{
    public Vector3 pA = new Vector3 (1, 0, 4);
    public Vector3 pB = new Vector3 (-1, 1, 3);
    public Vector3 pC = new Vector3 (0, 5, 4);
    public Vector3 pD = new Vector3 (1, 2, 2);

    void Start()
    {
        Vector3 S1 = pA;
        Vector3 S2 = pC;

        Vector3 V1 = vectorBA(pB, pA);
        Vector3 V2 = vectorBA(pD, pC);
        Vector3 S = vectorBA(S1, S2);

        float V1V1 = Vector3.Dot(V1, V1);
        float V2V2 = Vector3.Dot(V2, V2);
        float V1V2 = Vector3.Dot(V1, V2);
        float SS = Vector3.Dot(S, S);

        float[,] A =
        {
            { V1V1, -V1V2 },
            { -V1V2, V2V2 }
        };
        float[,] b =
        {
            { -Vector3.Dot(S, V1)},
            { Vector3.Dot(S, V2)}
        };

        float[,] invA = inverseA(A);

        float[,] t = MultiplyMatrices(invA, b);

        Debug.Log(t[0,0]);
        Debug.Log(t[1,0]);
    }

    float[,] MultiplyMatrices(float[,] mat1, float[,] mat2)
    {
        float[,] result = new float[2, 1];
        result[0, 0] = mat1[0, 0] * mat2[0, 0] + mat1[0, 1] * mat2[1, 0];
        result[1, 0] = mat1[1, 0] * mat2[0, 0] + mat1[1, 1] * mat2[1, 0];
        return result;
    }

    float[,] inverseA(float[,] A)
    {
        float detA = A[0, 0] * A[1, 1] - A[0, 1] * A[1, 0];
        if (detA == 0)
        {
            throw new System.Exception("Matrix is not invertible");
        }

        float[,] inverseA = new float[2, 2];
        inverseA[0, 0] = A[1, 1] / detA;
        inverseA[0, 1] = -A[0, 1] / detA;
        inverseA[1, 0] = -A[1, 0] / detA;
        inverseA[1, 1] = A[0, 0] / detA;

        return inverseA;
    }


    public Vector3 vectorBA(Vector3 A, Vector3 B)
    {
        return new Vector3(B.x - A.x, B.y - A.y, B.z - A.z);
    }
}
