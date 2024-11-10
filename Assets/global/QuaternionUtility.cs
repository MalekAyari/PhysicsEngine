using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class QuaternionUtility
{
    public static Quaternion CreateQuaternion(Vector3 axis, float angle)
    {
        // Normaliser l'axe de rotation
        axis.Normalize();

        float halfAngle = angle / 2;
        float sinHalfAngle = Mathf.Sin(halfAngle);

        return new Quaternion(
            axis.x * sinHalfAngle, 
            axis.y * sinHalfAngle, axis.z * sinHalfAngle, 
            Mathf.Cos(halfAngle)
            );
    }

    public static Matrix4x4 CreateRotationMatrixFromQuaternion(Quaternion q)
    {
        Matrix4x4 matrix = new Matrix4x4();

        // Calculer les �l�ments de la matrice de rotation
        float xx = q.x * q.x;        float xy = q.x * q.y;        float xz = q.x * q.z;
        float xw = q.x * q.w;        float yy = q.y * q.y;        float yz = q.y * q.z;
        float yw = q.y * q.w;        float zz = q.z * q.z;        float zw = q.z * q.w;

        matrix.m00 = 1 - 2 * (yy + zz);         matrix.m01 = 2 * (xy - zw);            matrix.m02 = 2 * (xz + yw);          matrix.m03 = 0;
        matrix.m10 = 2 * (xy + zw);             matrix.m11 = 1 - 2 * (xx + zz);        matrix.m12 = 2 * (yz - xw);          matrix.m13 = 0;
        matrix.m20 = 2 * (xz - yw);             matrix.m21 = 2 * (yz + xw);            matrix.m22 = 1 - 2 * (xx + yy);      matrix.m23 = 0;
        matrix.m30 = 0;                         matrix.m31 = 0;                        matrix.m32 = 0;                      matrix.m33 = 1;
        return matrix;
    }

    public static Vector3 MultiplyPoint3x4(Matrix4x4 matrix, Vector3 point)
    {
        Vector3 result;
        result.x = matrix.m00 * point.x + matrix.m01 * point.y + matrix.m02 * point.z + matrix.m03;
        result.y = matrix.m10 * point.x + matrix.m11 * point.y + matrix.m12 * point.z + matrix.m13;
        result.z = matrix.m20 * point.x + matrix.m21 * point.y + matrix.m22 * point.z + matrix.m23;
        return result;
    }
}