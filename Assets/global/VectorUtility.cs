using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorUtility
{
    public static Vector3 vectorBA(Vector3 A, Vector3 B)
    {
        return new Vector3(B.x-A.x, B.y-A.y, B.z-A.z);
    }

    public static float normBA(Vector3 vecBA)
    {
        return Mathf.Sqrt(Mathf.Pow(vecBA.x, 2) + Mathf.Pow(vecBA.y, 2) + Mathf.Pow(vecBA.z, 2));
    }
}