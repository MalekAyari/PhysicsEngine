using UnityEngine;

public class ClickForceApplier : MonoBehaviour
{
    public float forceMagnitude = 10f; // The magnitude of the force applied on click

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                CustomRB customRB = hit.collider.GetComponent<CustomRB>();
                CustomRBCube customRBCube = hit.collider.GetComponent<CustomRBCube>();

                if (customRB != null && customRBCube != null)
                {
                    // Get the hit point in world coordinates
                    Vector3 hitPoint = hit.point;
                    Debug.Log("Clicked on point: " + hitPoint);

                    // Calculate force vector
                    Vector3 forceVector = ray.direction * forceMagnitude;

                    // Calculate vector from Center of Mass to the hit point
                    Vector3 r = hitPoint - customRB.CenterOfMass;

                    // Calculate torque (r x F)
                    Vector3 torque = Vector3.Cross(r, forceVector);

                    // Apply the force and torque to CustomRB
                    customRB.appliedForce = forceVector;
                    customRB.appliedForcePoint = hitPoint;
                    Debug.Log(forceVector);
                    Debug.Log(hitPoint);
                }
            }
        }
    }
}
