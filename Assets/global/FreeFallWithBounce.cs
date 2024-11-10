using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeFallWithBounce : MonoBehaviour
{
    public Vector3 velocity; 
    public float gravity = 9.81f; 
    public float dt = 0.002f; 
    public Vector3 position; 
    public float mass = 0.5f; 
    public float frictionCoefficient = 100.0f; 
    private CustomCube customCube; 
    public float restitution;
    public float timeStep;

    // Start is called before the first frame update
    void Start()
    {
        position = new Vector3(0, 10, 0);
        velocity = Vector3.zero;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 acceleration = ComputeAcceleration(position, velocity);
        RK4Integration(acceleration);

        if (position.y <= 0.0f){
            HandleCollision();
        }

        transform.position = position;
    }

    Vector3 ComputeAcceleration(Vector3 pos, Vector3 vel){
        Vector3 gravityForce = mass * new Vector3(0, gravity, 0);
        Vector3 frictionForce = -frictionCoefficient * vel;
        return (gravityForce + frictionForce) / mass;
    }

    void RK4Integration(Vector3 acceleration){
        Vector3 k1_v = acceleration * timeStep;
        Vector3 k1_x = velocity * timeStep;

        Vector3 k2_v = ComputeAcceleration(position + k1_x * 0.5f, velocity + k1_v * 0.5f);
        Vector3 k2_x = (velocity + k1_v * 0.5f) * timeStep;
        
        Vector3 k3_v = ComputeAcceleration(position + k2_x * 0.5f, velocity + k2_v * 0.5f);
        Vector3 k3_x = (velocity + k2_v * 0.5f) * timeStep;

        Vector3 k4_v = ComputeAcceleration(position + k3_x * 0.5f, velocity + k3_v * 0.5f);
        Vector3 k4_x = (velocity + k3_v * 0.5f) * timeStep;
    }

    void HandleCollision(){
        position.y = 0;
        velocity.y = -restitution * velocity.y;
    }
}
