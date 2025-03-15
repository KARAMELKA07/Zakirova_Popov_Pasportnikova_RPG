using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public Transform target; // The object to orbit around
    public float distance = 5.0f; // Distance from the target
    public float rotationSpeed = 5.0f; // Rotation speed
    public float minYAngle = -20f; // Minimum vertical angle
    public float maxYAngle = 80f; // Maximum vertical angle
    public float initialHeightOffset = 2.0f; // Initial height offset

    private float rotationX = 0.0f;
    private float rotationY = 0.0f;

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("CameraOrbit: No target assigned.");
            return;
        }

        Vector3 angles = transform.eulerAngles;
        rotationX = angles.y;
        rotationY = angles.x;

        // Apply initial height offset
        transform.position = new Vector3(transform.position.x, transform.position.y + initialHeightOffset, transform.position.z);
    }

    void LateUpdate()
    {
        if (target == null)
            return;

        // Get mouse input
        rotationX += Input.GetAxis("Mouse X") * rotationSpeed;
        rotationY -= Input.GetAxis("Mouse Y") * rotationSpeed;

        // Clamp vertical rotation
        rotationY = Mathf.Clamp(rotationY, minYAngle, maxYAngle);

        // Calculate new position and rotation
        Quaternion rotation = Quaternion.Euler(rotationY, rotationX, 0);
        Vector3 position = target.position - (rotation * Vector3.forward * distance);
        position.y += initialHeightOffset;

        // Apply to camera
        transform.rotation = rotation;
        transform.position = position;
    }
}
