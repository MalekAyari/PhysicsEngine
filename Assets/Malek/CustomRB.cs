using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(CustomRBCube))]
public class CustomRB : MonoBehaviour
{
    [Header("Properties")]
    public float mass = 0;
    public Vector3 position;
    public Vector3 CenterOfMass;

    [Header("Physics properties")]
    public float gravity = 9.81f;
    public float Dt = 0.2f;

    [Header("Velocities")]
    public Vector3 velocity;
    public Vector3 angularVelocity;
    Vector3[] vector3s;
    CustomRBCube cube;

    public Vector3 appliedForce;
    public Vector3 appliedForcePoint;
    void Start()
    {
        cube = GetComponent<CustomRBCube>();
        
        velocity = Vector3.zero;
        position = Vector3.zero;
        
        //Initiate mass as sum of point weights
        foreach (vertex v in cube.verts){
            mass += v.weight;
        }
    }

    void FixedUpdate()
    {
        CenterOfMass = CalculateCenterOfMass(cube);

        vector3s = CalculateVectors(cube.vertices, CenterOfMass);
    }

    Vector3 CalculateCenterOfMass(CustomRBCube cube)
    {
        List<vertex> verts = cube.verts;

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

    //Calcule le vecteur système comme la somme des vecteurs de ses points
    Vector3 CalculerPSys(Vector3[] vectors){
        Vector3 Psys = Vector3.zero;

        for (int i = 0; i < vectors.Length; i++){
            Psys += cube.verts[i].weight * vectors[i];
        }

        return Psys; 
    }

    Vector3[] CalculateVectors( Vector3[] points, Vector3 com){
        Vector3[] vector3s = new Vector3[points.Count()];

        for (int i = 0; i < points.Count(); i++){
            vector3s[i] = VectorUtility.vectorBA(com, points[i]);
        }

        return vector3s;
    }
}