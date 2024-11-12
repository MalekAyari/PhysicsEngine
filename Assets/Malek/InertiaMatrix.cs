using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InertiaMatrix
{ 
    public Vector3 position;            // x(t)
    public float[,] rotationMatrix;     // R(t)
    public Vector3 linearMomentum;      // P(t)
    public Vector3 angularMomentum;     // L(t)

    public Vector3 Iinv;                // I-1
    public Vector3 velocity;            // v(t)
    public Vector3 omega;               // w(t)

    //Rigidbody
    public CustomRB rb;

    //Timestep
    public float dt;

    public InertiaMatrix(Vector3 x0, float[,] R0, Vector3 P0, Vector3 L0, float Timestep, CustomRB rigidBody)
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
        position = MatrixUtility.VectorDotMatrix(position, rotationMatrix);
    }

    void CalculateRotationMatrix(Vector3 w, Vector3 torque){
        float[,] wMat = MatrixUtility.Star(w);

        rotationMatrix = MatrixUtility.MatrixDotMatrix(wMat, rotationMatrix);

        var (newR, newW) = RK4Utility.RK4RotationMotion(rotationMatrix, w, MatrixUtility.Inverse(rb.cube.Ibody), torque, dt);
    }

    
    void CalculateLinearMomentum(Vector3 F){
        linearMomentum += F * dt;
    }

    void CalculateAngularMomentum(Vector3 torque){
        angularMomentum += torque * dt;
    }

    public void CalculateMatrix(Vector3 vt, Vector3 w, Vector3 F, Vector3 torque){
        //State
        CalculatePosition(vt);
        CalculateRotationMatrix(w, torque);
        CalculateLinearMomentum(F);
        CalculateAngularMomentum(torque);

        //Derivatives
        CalculateVelocity();
        CalculateIinv();
        CalculateOmega();
    }

    //ω(t) = I−1(t).L(t)
    public void CalculateOmega(){
        omega = Vector3.Cross(Iinv, angularMomentum);
    }

    // I−1(t) = R(t).I−1.bodyR(t)T
    public void CalculateIinv(){
        Iinv = MatrixUtility.Vector3FromMatrix(MatrixUtility.MatrixDotMatrix(rotationMatrix, MatrixUtility.MatrixDotMatrix(rb.cube.Ibodyinv, MatrixUtility.Transpose(rotationMatrix))));
    }

    //v(t) = P(t) / M
    public void CalculateVelocity(){
        velocity = linearMomentum / rb.mass;
    }
}