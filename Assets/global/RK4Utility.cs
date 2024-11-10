using UnityEngine;

public static class RK4Utility
{
    public static (Vector3, Vector3) RK4SolverMethod(Vector3 pos, Vector3 vel, Vector3 acc, float dt)
    {
        Vector3 k1 = dt * vel;
        Vector3 l1 = dt * acc;

        Vector3 k2 = dt * (vel + 0.5f * l1);
        Vector3 l2 = dt * (acc + 0.5f * l1);

        Vector3 k3 = dt * (vel + 0.5f * l2);
        Vector3 l3 = dt * (acc + 0.5f * l2);

        Vector3 k4 = dt * (vel + l3);
        Vector3 l4 = dt * (acc + l3);

        Vector3 newPos = pos + (k1 + 2 * k2 + 2 * k3 + k4) / 6.0f;
        Vector3 newVel = vel + (l1 + 2 * l2 + 2 * l3 + l4) / 6.0f;

        return (newPos, newVel);
    }

    // Integrates position and velocity
    public static (Vector3, Vector3) RK4LinearMotion(Vector3 pos, Vector3 vel, Vector3 acc, float dt)
    {
        Vector3 k1_pos = dt * vel;
        Vector3 l1_vel = dt * acc;

        Vector3 k2_pos = dt * (vel + 0.5f * l1_vel);
        Vector3 l2_vel = dt * (acc + 0.5f * l1_vel);

        Vector3 k3_pos = dt * (vel + 0.5f * l2_vel);
        Vector3 l3_vel = dt * (acc + 0.5f * l2_vel);

        Vector3 k4_pos = dt * (vel + l3_vel);
        Vector3 l4_vel = dt * (acc + l3_vel);

        Vector3 newPos = pos + (k1_pos + 2 * k2_pos + 2 * k3_pos + k4_pos) / 6.0f;
        Vector3 newVel = vel + (l1_vel + 2 * l2_vel + 2 * l3_vel + l4_vel) / 6.0f;

        return (newPos, newVel);
    }

    // Integrates rotation matrix and angular velocity
    public static (float[,], Vector3) RK4RotationMotion(float[,] R, Vector3 w, float[,] I_inv, Vector3 torque, float dt)
    {
        // Calculate skew-symmetric matrix for angular velocity
        float[,] Omega = MatrixUtility.VectorToSkewSymmetricMatrix(w);

        // k1 terms for rotation matrix and angular velocity
        float[,] k1_R = MatrixUtility.MatrixDotScalar(MatrixUtility.MatrixDotMatrix(R, Omega), dt);
        Vector3 k1_w = dt * MatrixUtility.MatrixDotVector(I_inv, torque);

        // k2 terms
        float[,] k2_R = MatrixUtility.MatrixDotScalar(MatrixUtility.MatrixDotMatrix(MatrixUtility.MatrixPlusMatrix(R, MatrixUtility.MatrixDotScalar(k1_R, 0.5f)), Omega), dt);
        Vector3 k2_w = dt * MatrixUtility.MatrixDotVector(I_inv, torque + 0.5f * k1_w);

        // k3 terms
        float[,] k3_R = MatrixUtility.MatrixDotScalar(MatrixUtility.MatrixDotMatrix(MatrixUtility.MatrixPlusMatrix(R, MatrixUtility.MatrixDotScalar(k2_R, 0.5f)), Omega), dt);
        Vector3 k3_w = dt * MatrixUtility.MatrixDotVector(I_inv, torque + 0.5f * k2_w);

        // k4 terms
        float[,] k4_R = MatrixUtility.MatrixDotScalar(MatrixUtility.MatrixDotMatrix(MatrixUtility.MatrixPlusMatrix(R, MatrixUtility.MatrixDotScalar(k3_R, 0.5f)), Omega), dt);
        Vector3 k4_w = dt * MatrixUtility.MatrixDotVector(I_inv, torque + k3_w);

        // Update rotation matrix and angular velocity using RK4 integration
        float[,] newR = MatrixUtility.MatrixPlusMatrix(
            R, 
            MatrixUtility.MatrixDotScalar(
                MatrixUtility.MatrixPlusMatrix(
                    MatrixUtility.MatrixPlusMatrix(
                        MatrixUtility.MatrixPlusMatrix(
                            k1_R, MatrixUtility.MatrixDotScalar(
                                k2_R, 
                                2
                            )
                        ), 
                        MatrixUtility.MatrixDotScalar(k3_R, 2)
                    ), 
                    k4_R
                ), 
                1/6.0f
            )
        );
        Vector3 newW = w + (k1_w + 2 * k2_w + 2 * k3_w + k4_w) / 6.0f;

        return (newR, newW);
    }
}
