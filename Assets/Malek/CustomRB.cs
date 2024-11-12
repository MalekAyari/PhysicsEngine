using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;

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
    public CustomRBCube cube;
    public InertiaMatrix state;

    public float tn;
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

        //Initialises mass as sum of point weights
        for (int i=0; i<cube.verts.Count; i++)
        {
            mass += cube.verts[i].weight;
        }

        //Initialises X(t) = (x(t), R(t), P(t), L(t))
        state = new InertiaMatrix(cube.worldCenterOfMass, rotationMatrix, Vector3.zero, Vector3.zero, Dt, this);
    }

    //Moment t=t+1
    void FixedUpdate()
    {
        tn += Time.fixedDeltaTime;

        // Compute force and torque
        Vector3 appliedForce = Vector3.down * gravity * mass;

        Vector3 appliedForcePoint = cube.worldCenterOfMass;

        Vector3 torque = Vector3.Cross(appliedForcePoint - cube.worldCenterOfMass, appliedForce);

        // Apply RK4 integration to update the state
        RK4Utility.RK4Rigidbody(state, this, Dt, out state.position, out state.velocity, out state.rotationMatrix, out state.omega);

        // Update derived properties based on the new state
        state.CalculateVelocity();
        state.CalculateIinv();
        state.CalculateOmega();

        // Update center of mass and angular velocity if needed
        cube.worldCenterOfMass = state.position;
        angularVelocity = state.omega;


        for (int i = 0; i < cube.verts.Count; i++)
        {
            vertex v = cube.verts[i];
            Vector3 point = cube.mesh.vertices[i];
        }

        foreach (vertex v in cube.verts)
        {
            // Convert each vertex's local position to world position
            Vector3 rotatedPosition = MatrixUtility.VectorDotMatrix(v.localPosition, state.rotationMatrix);
            v.position = rotatedPosition + cube.worldCenterOfMass;
        }
    }

    

}