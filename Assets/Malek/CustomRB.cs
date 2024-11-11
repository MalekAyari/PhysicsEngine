using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms;

[RequireComponent(typeof(CustomRBCube))]
public class CustomRB : MonoBehaviour
{
    [Header("Properties")]
    public float mass = 0;

    [Header("Physics properties")]
    public float gravity = 9.81f;
    public float friction = 0;
    public float Dt = 0.2f;


    [Header("Velocities")]
    public Vector3 acceleration;
    public Vector3 velocity;
    public Vector3 angularVelocity;

    [Header("Information")]
    public Vector3 inertiaTensor;
    public Vector3 inertiaTensorRotation;

    //Object system
    public CustomRBCube cube;
    
    MovementMatrix State;

    
    
    void Start()
    {
        cube = GetComponent<CustomRBCube>();

        velocity = Vector3.zero;
        
        //Angular velocity = W
        angularVelocity = Vector3.zero;

        float[,] rotationMatrix = new float[,]
        {
            {1, 0, 0},
            {0, 1, 0},
            {0, 0, 1}
        };

        //Basic data

        //Initialises X(t) = (x(t), R(t), P(t), L(t))
        State = new MovementMatrix(cube.worldCenterOfMass, rotationMatrix, Vector3.zero, Vector3.zero, Dt, this);

        //Initiate mass as sum of point weights
        foreach (vertex v in cube.verts){
            mass += v.weight;
        }

    }

    void FixedUpdate()
    {
        float[,] inertiaTensor = CalculateInertiaTensor(State.rotationMatrix, cube.Ibody);

        Vector3 appliedForce = Vector3.zero;
        Vector3 appliedForcePoint = Vector3.zero;

        //Apply force here
        //===================

        //Update values
        Vector3 torque = Vector3.Cross(appliedForcePoint - cube.localCenterOfMass, appliedForce);

        foreach (vertex p in cube.verts){
            Vector3 localPos = p.position-cube.localCenterOfMass;
            p.position = MatrixUtility.MatrixDotVector(State.rotationMatrix, localPos) + cube.worldCenterOfMass;

            //new_ri(t) = angularVelocity x (ri(t) - localCenterOfMass) + velocity
        }
        
        State.updateMatrix(velocity, angularVelocity, velocity, torque);
    }

    float[,] CalculateRotationMatrix(float[,] inertiaTensor, Vector3 appliedForce, Vector3 appliedForcePoint)
    {
        Vector3 moment = Vector3.Cross(appliedForcePoint - cube.localCenterOfMass, appliedForce);

        Vector3 angularVelocity = MatrixUtility.MatrixDotVector(inertiaTensor, moment);

        Vector3 axis = angularVelocity.normalized;
        float angle = angularVelocity.magnitude * Dt;

        Quaternion rotationQuaternion = QuaternionUtility.CreateQuaternion(axis, angle);

        return QuaternionUtility.CreateRotationMatrixFromQuaternion(rotationQuaternion);
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

    //Calculates I = R(t).Ibody.R(t)T
    float[,] CalculateInertiaTensor(float[,] rotationMatrix, float[,] Ibody){
        return MatrixUtility.MatrixDotMatrix(MatrixUtility.MatrixDotMatrix(rotationMatrix, Ibody), MatrixUtility.Transpose(rotationMatrix));
    }
    
    //Calculates W = I-1.L(t)
    public Vector3 CalculateAngularVelocity(
        float[,] inertiaTensor, 
        Vector3 appliedForce, 
        Vector3 appliedForcePosition, 
        Vector3 pointPosition, 
        Vector3 angularVelocity, 
        float deltaTime
    )
    {
        //Torque
        Vector3 r = pointPosition - appliedForcePosition;
        Vector3 torque = Vector3.Cross(r, appliedForce);

        //Angular acceleration
        Vector3 angularAcceleration = CalculateAngularAcceleration(inertiaTensor, torque);

        //Update the angular velocity
        Vector3 newAngularVelocity = angularVelocity + angularAcceleration * deltaTime;

        return newAngularVelocity;
    }

    public Vector3 CalculateAngularAcceleration(float[,] State, Vector3 torque)
    {
        Vector3 angularAcceleration = MatrixUtility.MatrixDotVector(cube.Ibodyinv, torque);

        return angularAcceleration;
    }

}