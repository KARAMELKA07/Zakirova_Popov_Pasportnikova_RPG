using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public Transform target; 
    public float distance = 5.0f; 
    public float rotationSpeed = 5.0f; 
    public float minYAngle = -20f; 
    public float maxYAngle = 80f; 
    public float initialHeightOffset = 2.0f; 

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
      
        transform.position = new Vector3(transform.position.x, transform.position.y + initialHeightOffset, transform.position.z);
    }

    void LateUpdate()
    {
        if (target == null)
            return;

        rotationX += Input.GetAxis("Mouse X") * rotationSpeed;
        rotationY -= Input.GetAxis("Mouse Y") * rotationSpeed;

        rotationY = Mathf.Clamp(rotationY, minYAngle, maxYAngle);

        Quaternion rotation = Quaternion.Euler(rotationY, rotationX, 0);
        Vector3 position = target.position - (rotation * Vector3.forward * distance);
        position.y += initialHeightOffset;

        transform.rotation = rotation;
        transform.position = position;
    }
}
