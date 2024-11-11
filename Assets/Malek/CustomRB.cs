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
    public Vector3 linearVelocity;
    public Vector3 angularVelocity;

    [Header("Information")]
    public Vector3 inertiaTensor;
    public Vector3 inertiaTensorRotation;

    //Object system
    public CustomRBCube cube;
    
    MovementMatrix State;

    
    //Moment t=0
    void Start()
    {
        cube = GetComponent<CustomRBCube>();

        linearVelocity = Vector3.zero;
        
        //Angular velocity = W
        angularVelocity = Vector3.zero;

        float[,] rotationMatrix = new float[,]
        {
            {1, 0, 0},
            {0, 1, 0},
            {0, 0, 1}
        };

        //Initialises X(t) = (x(t), R(t), P(t), L(t))
        State = new MovementMatrix(cube.worldCenterOfMass, rotationMatrix, Vector3.zero, Vector3.zero, Dt, this);

        //Initiate mass as sum of point weights
        foreach (vertex v in cube.verts){
            mass += v.weight;
        }
    }

    //Moment t=t+1
    void FixedUpdate()
    {
        Vector3 appliedForce = Vector3.zero;
        Vector3 appliedForcePoint = Vector3.zero;

        //Apply force here
        
        //===================

        //Update values
        Vector3 torque = Vector3.Cross(appliedForcePoint - cube.localCenterOfMass, appliedForce);

        State.updateMatrix(linearVelocity, angularVelocity, appliedForce, torque);
    }

}