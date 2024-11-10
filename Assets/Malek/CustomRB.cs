using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(CustomRBCube))]
public class CustomRB : MonoBehaviour
{
    [Header("Properties")]
    public float mass = 0;
    public Vector3 position;

    [Header("Center Of Mass")]
    public Vector3 LocalCenterOfMass;
    public Vector3 GlobalCenterOfMass;

    [Header("Physics properties")]
    public float gravity = 9.81f;
    public float friction = 0;
    public float Dt = 0.2f;

    [Header("Velocities")]
    public Vector3 velocity;
    public Vector3 angularVelocity;

    //Object system
    CustomRBCube cube;

    


    //Maths==================================================================
    //Rotational state of the system
    float[,] rotationMatrix = new float[,]
        {
            {1, 0, 0},
            {0, 1, 0},
            {0, 0, 1}
        };
    
    //Point vectors
    Vector3[] pointVectorsFromCenterOfMass;
    
    //For points
    InertiaMatrix localInertiaMatrix;

    //For system
    InertiaMatrix globalInertiaMatrix;
    //=======================================================================
    
    void Start()
    {
        cube = GetComponent<CustomRBCube>();
        GlobalCenterOfMass = LocalCenterOfMass;
        velocity = Vector3.zero;
        position = Vector3.zero;

        localInertiaMatrix = new InertiaMatrix(LocalCenterOfMass, rotationMatrix, Vector3.zero, Vector3.zero, Dt, cube);
        globalInertiaMatrix = new InertiaMatrix(GlobalCenterOfMass, rotationMatrix, Vector3.zero, Vector3.zero, Dt, cube);

        //Initiate mass as sum of point weights
        foreach (vertex v in cube.verts){
            mass += v.weight;
        }
    }

    void FixedUpdate()
    {
        LocalCenterOfMass = CalculateCenterOfMass(cube);
        
        float[,] inertiaTensor = CalculateInertiaTensor(rotationMatrix, cube.Ibody);

        Vector3 appliedForce = Vector3.zero;
        Vector3 appliedForcePoint = Vector3.zero;
        //Apply force here

        rotationMatrix = CalculateRotationMatrix(inertiaTensor, appliedForcePoint, appliedForce);



        pointVectorsFromCenterOfMass = CalculateVectors(cube.vertices, LocalCenterOfMass);
    }

    float[,] CalculateRotationMatrix(float[,] inertiaTensor, Vector3 appliedForce, Vector3 appliedForcePoint)
    {
        Vector3 moment = Vector3.Cross(appliedForcePoint - LocalCenterOfMass, appliedForce);

        Vector3 angularVelocity = MatrixUtility.MatrixDotVector(inertiaTensor, moment);

        Vector3 axis = angularVelocity.normalized;
        float angle = angularVelocity.magnitude * Dt;

        Quaternion rotationQuaternion = QuaternionUtility.CreateQuaternion(axis, angle);

        return QuaternionUtility.CreateRotationMatrixFromQuaternion(rotationQuaternion);
    }
    // void UpdateRotationMatrix(float[,] inertiaTensor, Vector3 appliedForce, Vector3 appliedForcePoint){
    //     Vector3 moment = Vector3.Cross(VectorUtility.vectorBA(LocalCenterOfMass, appliedForcePoint), appliedForce);
    //     Vector3 rotationAxis = moment.normalized;

    //     Vector3 angularAcceleration = CalculateAngularVelocity(inertiaTensor, moment);
    //     angularVelocity += angularAcceleration * Dt;

    //     rotationMatrix = MatrixUtility.UpdateRotationMatrix(rotationMatrix, angularVelocity, Dt);
    // }

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
    Vector3 CalculerPSys(Vector3[] vectors)
    {
        Vector3 Psys = Vector3.zero;

        for (int i = 0; i < vectors.Length; i++){
            Psys += cube.verts[i].weight * vectors[i];
        }

        return Psys; 
    }

    //Calcule les vecteurs entre tout point et centre de masse pour maintenir
    //une distance égale d'une image à la suivante 
    Vector3[] CalculateVectors(Vector3[] points, Vector3 com)
    {
        Vector3[] pointVectorsFromCenterOfMass = new Vector3[points.Count()];

        for (int i = 0; i < points.Count(); i++){
            pointVectorsFromCenterOfMass[i] = VectorUtility.vectorBA(com, points[i]);
        }

        return pointVectorsFromCenterOfMass;
    }

    //Calculates I-1 = R(t).Ibody-1.R(t)T
    float[,] CalculateInertiaTensor(float[,] rotationMatrix, float[,] Ibody){
        return MatrixUtility.MatrixDotMatrix(MatrixUtility.MatrixDotMatrix(rotationMatrix, MatrixUtility.Inverse(Ibody)), MatrixUtility.Transpose(rotationMatrix));
    }

    //Calculates W = I-1.L(t)
    Vector3 CalculateAngularVelocity(float[,] InertiaTensor, Vector3 angularMomentum){
        return MatrixUtility.MatrixDotVector(InertiaTensor, angularMomentum);
    }

}