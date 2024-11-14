using UnityEngine;

public class CustomHingeJoint : MonoBehaviour
{
    public Transform connectedObject; // The other object to "hinge" to
    public Vector3 hingeAxis = Vector3.up; // Axis for hinge rotation
    public float targetAngle = 0f; // Target rotation angle
    public float hardLimitMin = -45f; // Minimum limit for rotation angle
    public float hardLimitMax = 45f; // Maximum limit for rotation angle
    public float damping = 0.1f; // Damping factor for smoother movement

    private float currentAngle = 0f;
    private float angularVelocity = 0f;


    private bool isRotating = false; // Flag to track if the mouse is held down
    private float lastMousePositionX = 0f; // Last recorded mouse X position
    private float rotationSpeed = 0.05f; // How fast the object rotates
    public bool isHinge;

    
    private Mesh mesh;
    private Vector3[] originalVertices; // Original vertices for reference
    private Vector3[] rotatedVertices;  // Vertices after rotation


    void Update()
    {
        if(isHinge)
        {
            if (Input.GetAxis("Horizontal")>0)
            {
                targetAngle += rotationSpeed;
            }


            if (Input.GetAxis("Horizontal") < 0)
            {
                targetAngle -= rotationSpeed;
            }
        }
        

        // Calculate difference to the target angle
        float angleDiff = targetAngle - currentAngle;

        // Apply damping to smooth out movement
        angleDiff *= (1f - damping);

        // Apply hard limits to prevent over-rotation
        angleDiff = Mathf.Clamp(angleDiff, hardLimitMin - currentAngle, hardLimitMax - currentAngle);

        // Update current angle and apply rotation
        currentAngle += angleDiff * Time.deltaTime;
        transform.localRotation = Quaternion.AngleAxis(currentAngle, hingeAxis);

        // Calculate relative position correction
        if (connectedObject != null)
        {
            Vector3 offset = connectedObject.position - transform.position;
            if (offset.magnitude > 1f) // Example threshold for max distance
            {
                Vector3 correction = offset.normalized * (offset.magnitude - 2f);
                transform.position += correction * Time.deltaTime;
            }
        }
    }




}

