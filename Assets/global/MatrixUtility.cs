using UnityEngine;

public static class MatrixUtility 
{

    public static float[,] Transpose(float[,] matrix)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);
        float[,] transposedMatrix = new float[cols, rows];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                transposedMatrix[j, i] = matrix[i, j];
            }
        }

        return transposedMatrix;
    }

    public static float[,] Inverse(float[,] matrix)
    {
        int n = matrix.GetLength(0);
        float[,] augmentedMatrix = new float[n, 2 * n];

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                augmentedMatrix[i, j] = matrix[i, j];
                augmentedMatrix[i, j + n] = (i == j) ? 1 : 0;
            }
        }

        for (int i = 0; i < n; i++)
        {
            int maxRow = i;
            for (int j = i + 1; j < n; j++)
            {
                if (Mathf.Abs(augmentedMatrix[j, i]) > Mathf.Abs(augmentedMatrix[maxRow, i]))
                {
                    maxRow = j;
                }
            }

            for (int j = 0; j < 2 * n; j++)
            {
                float temp = augmentedMatrix[i, j];
                augmentedMatrix[i, j] = augmentedMatrix[maxRow, j];
                augmentedMatrix[maxRow, j] = temp;
            }

            float pivot = augmentedMatrix[i, i];
            for (int j = 0; j < 2 * n; j++)
            {
                augmentedMatrix[i, j] /= pivot;
            }

            for (int j = i + 1; j < n; j++)
            {
                float factor = augmentedMatrix[j, i];
                for (int k = 0; k < 2 * n; k++)
                {
                    augmentedMatrix[j, k] -= augmentedMatrix[i, k] * factor;
                }
            }
        }

        for (int i = n - 1; i >= 0; i--)
        {
            for (int j = i - 1; j >= 0; j--)
            {
                float factor = augmentedMatrix[j, i];
                for (int k = 0; k < 2 * n; k++)
                {
                    augmentedMatrix[j, k] -= augmentedMatrix[i, k] * factor;
                }
            }
        }

        float[,] inverseMatrix = new float[n, n];
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                inverseMatrix[i, j] = augmentedMatrix[i, j + n];
            }
        }

        return inverseMatrix;
    }

    public static float[,] ScalarToMatrix(float num){
        return new float[,] 
            {
                {num, 0, 0},
                {0, num, 0},
                {0, 0, num},
            };
    }

    public static float[,] VectorToSkewSymmetricMatrix(Vector3 w){
        return new float[,]
            {
                {0,     -w.z,   w.y },
                {w.z,   0,      -w.x},
                {-w.y,  w.x,    0   },
            };
    }
    
    public static float[,] Vector3ToMatrix(Vector3 vector)
    {
        return new float[,]
        {
            { 0, -vector.z, vector.y },
            { vector.z, 0, -vector.x },
            { -vector.y, vector.x, 0 }
        };
    }

    public static float[,] MatrixPlusMatrix(float[,] A, float[,] B)
    {
        int rows = A.GetLength(0);
        int cols = A.GetLength(1);

        // Ensure matrices A and B have the same dimensions
        if (rows != B.GetLength(0) || cols != B.GetLength(1))
        {
            throw new System.ArgumentException("Matrices must have the same dimensions for addition.");
        }

        float[,] result = new float[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                result[i, j] = A[i, j] + B[i, j];
            }
        }

        return result;
    }

    public static float[,] MatrixPlusVector(float[,] matrix, Vector3 vector)
    {
        int rows = matrix.GetLength(0);
        int columns = matrix.GetLength(1);

        // Ensure the vector size matches the matrix dimensions
        if (rows != 3)
        {
            throw new System.ArgumentException("Matrix row size must match the vector length (3).");
        }

        float[,] result = new float[rows, columns];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                // Add the corresponding component of the vector to each row
                result[i, j] = matrix[i, j] + (i == 0 ? vector.x : i == 1 ? vector.y : vector.z);
            }
        }

        return result;
    }

    public static float[,] MatrixMinusMatrix(float[,] A, float[,] B)
    {
        int rows = A.GetLength(0);
        int cols = A.GetLength(1);
        if (B.GetLength(0) != rows || B.GetLength(1) != cols)
        {
            throw new System.ArgumentException("Matrix dimensions must match.");
        }

        float[,] result = new float[rows, cols];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                result[i, j] = A[i, j] - B[i, j];
            }
        }

        return result;
    }

    public static float[,] MatrixDotMatrix(float[,] A, float[,] B)
    {
        int rows = A.GetLength(0);
        int cols = B.GetLength(1);
        float[,] result = new float[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                result[i, j] = 0;
                for (int k = 0; k < A.GetLength(1); k++)
                {
                    result[i, j] += A[i, k] * B[k, j];
                }
            }
        }

        return result;
    }

    public static float[,] MatrixDotScalar(float[,] matrix, float scalar)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);
        float[,] result = new float[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                result[i, j] = matrix[i, j] * scalar;
            }
        }

        return result;
    }

    public static Vector3 MatrixDotVector(float[,] matrix, Vector3 vector)
    {
        Vector3 result = new Vector3();
        result.x = matrix[0, 0] * vector.x + matrix[0, 1] * vector.y + matrix[0, 2] * vector.z;
        result.y = matrix[1, 0] * vector.x + matrix[1, 1] * vector.y + matrix[1, 2] * vector.z;
        result.z = matrix[2, 0] * vector.x + matrix[2, 1] * vector.y + matrix[2, 2] * vector.z;

        return result;
    }

    public static Vector3 VectorDotMatrix(Vector3 point, float[,] matrix)
    {
        Vector3 result;
        result.x = matrix[0,0] * point.x + matrix[0,1] * point.y + matrix[0,2];
        result.y = matrix[1,0] * point.x + matrix[1,1] * point.y + matrix[1,2];
        result.z = matrix[2,0] * point.x + matrix[2,1] * point.y + matrix[2,2];
        return result;
    }

    public static Vector3 Vector3FromMatrix(float[,] matrix) 
    {
        if (matrix.GetLength(0) != 3 || matrix.GetLength(1) != 3) {
            Debug.Log("Matrix must be 3x3");
        }

        return new Vector3(matrix[0, 0], matrix[0, 1], matrix[0, 2]);
    }

    // Helper function to format a 2D array for logging
    public static string MatrixToString(float[,] matrix)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                sb.Append(matrix[i, j].ToString("F4")).Append("\t");
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }

   public static float Determinant(float[,] matrix)
    {
        if (matrix.GetLength(0) != 3 || matrix.GetLength(1) != 3)
        {
            Debug.Log("Must be a 3x3 matrix!");
        }

        float a = matrix[0, 0];
        float b = matrix[0, 1];
        float c = matrix[0, 2];
        float d = matrix[1, 0];
        float e = matrix[1, 1];
        float f = matrix[1, 2];
        float g = matrix[2, 0];
        float h = matrix[2, 1];
        float i = matrix[2, 2];

        return a * (e * i - f * h) - b * (d * i - f * g) + c * (d * h - e * g);
    }

    public static float[,] Orthogonalize(float[,] R)
    {
        // Extract columns of R
        Vector3 col1 = new Vector3(R[0, 0], R[1, 0], R[2, 0]);
        Vector3 col2 = new Vector3(R[0, 1], R[1, 1], R[2, 1]);
        Vector3 col3 = new Vector3(R[0, 2], R[1, 2], R[2, 2]);

        // Re-orthogonalize columns using Gram-Schmidt process
        col1 = col1.normalized;
        col2 = (col2 - Vector3.Dot(col2, col1) * col1).normalized;
        col3 = Vector3.Cross(col1, col2);  // Ensure orthogonality by cross product

        // Update R with orthogonalized columns
        R[0, 0] = col1.x; R[1, 0] = col1.y; R[2, 0] = col1.z;
        R[0, 1] = col2.x; R[1, 1] = col2.y; R[2, 1] = col2.z;
        R[0, 2] = col3.x; R[1, 2] = col3.y; R[2, 2] = col3.z;

        return R;
    }

}