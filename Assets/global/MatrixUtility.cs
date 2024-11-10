using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MatrixUtility 
{
    public static float[,] MultiplyMatrices(float[,] mat1, float[,] mat2)
    {
        float[,] result = new float[2, 1];
        result[0, 0] = mat1[0, 0] * mat2[0, 0] + mat1[0, 1] * mat2[1, 0];
        result[1, 0] = mat1[1, 0] * mat2[0, 0] + mat1[1, 1] * mat2[1, 0];
        return result;
    }

    public static float[,] inverseA(float[,] A)
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
}
