using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementMatrix
{ 
    public Vector3 position;
    public float[,] rotationMatrix;
    public Vector3 linearMomentum;
    public Vector3 angularMomentum;

    public Vector3 Iinv;
    public Vector3 velocity;
    public Vector3 omega;

    //Rigidbody
    public CustomRB rb;

    //Timestep
    public float dt;

    public MovementMatrix(Vector3 x0, float[,] R0, Vector3 P0, Vector3 L0, float Timestep, CustomRB rigidBody)
    {
        position = x0;
        rotationMatrix = R0;
        linearMomentum = P0;
        angularMomentum = L0;
        dt = Timestep;
        rb = rigidBody;
    }

    void updatePosition(Vector3 vt){
        position += vt * dt;
    }

    void updateRotationMatrix(Vector3 w, Vector3 torque){
        float[,] wMat = new float[,]
            {
                {0,     -w.z,   w.y },
                {w.z,   0,      -w.x},
                {-w.y,  w.x,    0   },
            };

        rotationMatrix = MatrixUtility.MatrixDotMatrix(wMat, rotationMatrix);

        var (newR, newW) = RK4Utility.RK4RotationMotion(rotationMatrix, w, MatrixUtility.Inverse(rb.cube.Ibody), torque, dt);
    }

    void updateLinearMomentum(Vector3 F){
        linearMomentum += F * dt;
    }

    void updateAngularMomentum(Vector3 torque){
        angularMomentum += torque * dt;
    }

    public void updateMatrix(Vector3 vt, Vector3 w, Vector3 F, Vector3 torque){
        updatePosition(vt);
        updateRotationMatrix(w, torque);
        updateLinearMomentum(F);
        updateAngularMomentum(torque);
    }

    public void CalculateOmega(){
        omega = Vector3.Cross(Iinv, angularMomentum);
    }

    public void CalculateIinv(){
        Iinv = MatrixUtility.Vector3FromMatrix(MatrixUtility.MatrixDotMatrix(rotationMatrix, MatrixUtility.MatrixDotMatrix(rb.cube.Ibodyinv, MatrixUtility.Transpose(rotationMatrix))));
    }

    public void CalculateVelocity(){
        velocity = linearMomentum / rb.mass;
    }
}