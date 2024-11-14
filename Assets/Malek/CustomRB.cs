using UnityEngine;

[RequireComponent(typeof(CustomRBCube))]
public class CustomRB : MonoBehaviour
{
    [Header("Properties")]
    public float mass = 0;

    [Header("Simulation Settings")]
    public float forceMagnitude = 10f;  //Minimum of 10 otherwise won't notice it
    public float gravity = 9.81f;
    public float friction = 0.1f;       // Friction coefficient
    public float angularFriction = 0.1f;
    public float Dt = 0.2f;

    [Header("Velocities")]
    public Vector3 acceleration;
    public Vector3 linearVelocity;
    public Vector3 angularVelocity;


    [Header("Information")]
    public Vector3 inertiaTensor;
    public CustomRBCube cube;
    public State state;
    public float tn;
    Vector3 appliedForce;
    Vector3 appliedForcePoint;

    void Start()
    {
        cube = GetComponent<CustomRBCube>();

        linearVelocity = Vector3.zero;
        angularVelocity = Vector3.zero;

        float[,] rotationMatrix = new float[,]
        {
            {1, 0, 0},
            {0, 1, 0},
            {0, 0, 1}
        };

        // Initialises mass as sum of point weights
        for (int i = 0; i < cube.verts.Count; i++)
        {
            mass += cube.verts[i].weight;
        }

        // Initialises X(t) = (x(t), R(t), P(t), L(t))
        state = new State(cube.worldCenterOfMass, rotationMatrix, Vector3.zero, Vector3.zero, Dt, this);

        appliedForce = new Vector3(0, 2, 0) * forceMagnitude;
        appliedForcePoint = (cube.verts[0].position + cube.verts[1].position) / 2;
    }

    void FixedUpdate()
    {
        tn += Time.fixedDeltaTime;

        // Compute force and torque
        Vector3 torque;
        Vector3 totalForce;

        (torque, totalForce) = CompileForces(appliedForce, appliedForcePoint);
        
        state.CalculateMatrix(totalForce, torque);

        // Update center of mass and angular velocity if needed
        cube.worldCenterOfMass += state.position;
        angularVelocity = state.omega;
        linearVelocity = state.velocity;
        acceleration = state.acceleration;
        inertiaTensor = MatrixUtility.Vector3FromMatrix(state.InertiaMatrix);
        cube.position = state.position;

        // Matrix4x4 translation = cube.CreateTranslationMatrix(state.position);
        // cube.ApplyTransformation(translation);
        ApplyVertexTransformation(state.omega);
        appliedForce = Vector3.zero;
        appliedForcePoint = Vector3.zero;
    }

    // r'i(t) = ω(t) × (ri(t) − x(t)) + v(t)
    void ApplyVertexTransformation(Vector3 omega)
    {
        for (int i = 0; i < cube.verts.Count; i++)
        {
            // Vector from the center of mass to the vertex position
            Vector3 r = cube.verts[i].position - cube.worldCenterOfMass;

            // Calculate the rotational effect
            Vector3 rotationalDisplacement = Vector3.Cross(omega, r);

            // Update the vertex position
            cube.verts[i].position += rotationalDisplacement * Dt + state.velocity * Dt;
        }

        // Updating the mesh
        Vector3[] updatedVertices = new Vector3[cube.verts.Count];
        for (int i = 0; i < cube.verts.Count; i++)
        {
            updatedVertices[i] = cube.verts[i].position;
        }
        cube.vertices = updatedVertices;
        cube.mesh.vertices = updatedVertices;
        cube.mesh.RecalculateBounds();
    }

    (Vector3, Vector3) CompileForces(Vector3 appliedForce, Vector3 appliedForcePoint)
    {
        // Gravity force
        Vector3 grav = Vector3.down * gravity / mass;

        Vector3 totalForce = grav + appliedForce; 

        Vector3 frictionForce = -friction * totalForce.normalized;

        // Torque due to applied force
        Vector3 torque = Vector3.Cross(appliedForcePoint - cube.localCenterOfMass, appliedForce);
        totalForce = grav + appliedForce - frictionForce;

        return (torque, totalForce);
    }
}