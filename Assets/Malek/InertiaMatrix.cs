using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{ 
    public Vector3 position;            // x(t)
    public float[,] rotationMatrix;     // R(t)
    public Vector3 linearMomentum;      // P(t)
    public Vector3 angularMomentum;     // L(t)

    public float[,] InertiaMatrix;      // I(t)
    public Vector3 velocity;            // v(t)
    public Vector3 acceleration;        // a(t)
    public Vector3 omega;               // w(t)

    //Rigidbody
    public CustomRB rb;

    //Timestep
    public float dt;

    public State(Vector3 x0, float[,] R0, Vector3 P0, Vector3 L0, float Timestep, CustomRB rigidBody)
    {
        position = x0;
        rotationMatrix = R0;
        linearMomentum = P0;
        angularMomentum = L0;
        dt = Timestep;
        rb = rigidBody;
    }

    void CalculatePosition(Vector3 vt){
        position += vt * dt;
    }

    void CalculateRotationMatrix(Vector3 w){
        float[,] wMat = MatrixUtility.Star(w);

        // rotationMatrix = MatrixUtility.MatrixPlusMatrix(rotationMatrix, MatrixUtility.MatrixDotScalar(MatrixUtility.MatrixDotMatrix(wMat, rotationMatrix), dt));
        MatrixUtility.MatrixDotMatrix(wMat, rotationMatrix);
    }
    
    void CalculateLinearMomentum(Vector3 F){
        linearMomentum += F * dt;
    }

    void CalculateAngularMomentum(Vector3 torque){
        angularMomentum += torque * dt;
    }

    public void CalculateMatrix(Vector3 force, Vector3 torque){
        //State
        CalculatePosition(velocity);
        CalculateRotationMatrix(omega);
        CalculateLinearMomentum(force);
        CalculateAngularMomentum(torque);

        //Derivatives
        CalculateInertiaMatrix();
        CalculateVelocity();
        CalculateOmega();
        CalculateAcceleration();
    }

    //ω(t) = I−1(t).L(t)
    public void CalculateOmega()
    {
        // Check if the inertia matrix is invertible
        float determinant = MatrixUtility.Determinant(InertiaMatrix);
        if (Mathf.Abs(determinant) < Mathf.Epsilon)
        {
            Debug.LogError("Inertia matrix is singular or nearly singular. Cannot invert.");
            return;
        }

        // Proceed with inversion
        omega = MatrixUtility.MatrixDotVector(MatrixUtility.Inverse(InertiaMatrix), angularMomentum);
    }

    // I(t) = R(t).Ibody.R(t)T
    public void CalculateInertiaMatrix(){
        if (rb.mass <= 0) Debug.LogError("mass is zero");
        InertiaMatrix = MatrixUtility.MatrixDotMatrix(MatrixUtility.MatrixDotMatrix(rotationMatrix, rb.cube.Ibody) , MatrixUtility.Transpose(rotationMatrix)) ;
    }

    // p += v * dt
    // v += acc * dt
    // acc = v / dt
    // v(t) = P(t) / M
    public void CalculateVelocity(){
        velocity = linearMomentum / rb.mass;
    }
    public void CalculateAcceleration(){
        acceleration = velocity / dt;
    }
}