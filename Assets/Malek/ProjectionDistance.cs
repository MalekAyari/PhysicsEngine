using System;
using UnityEngine;

[RequireComponent(typeof(CustomCube))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class ProjectionDistance : MonoBehaviour
{
    private Vector3 pointA = new Vector3 (1, 4, -5);
    private Vector3 pointB = new Vector3 (2, -3, 8);
    private Vector3 pointC = new Vector3 (2, 3, 6);

    void Start()
    {
        Vector3 CA = vectorBA(pointC, pointA);
        Vector3 projectionBA = projBA(CA, vectorBA(pointB, pointA));
        float distance = Mathf.Sqrt(Mathf.Pow(normBA(vectorBA(pointB, pointA)), 2) - Mathf.Pow(normBA(projectionBA), 2));
        Debug.Log(distance);
    }

    public float normBA(Vector3 vecBA)
    {
        return Mathf.Sqrt(Mathf.Pow(vecBA.x, 2) + Mathf.Pow(vecBA.y, 2) + Mathf.Pow(vecBA.z, 2));
    }

    public Vector3 vectorBA(Vector3 A, Vector3 B)
    {
        return new Vector3(B.x-A.x, B.y-A.y, B.z-A.z);
    }

    public Vector3 projBA(Vector3 cast, Vector3 BA) 
    {
        float BAV = Vector3.Dot(BA, cast);
        float VV = Vector3.Dot(cast, cast);
        return cast * BAV / VV;
    }

    
}
