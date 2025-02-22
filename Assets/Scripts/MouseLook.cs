using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public Transform playerTransform;  // Reference to the player's transform
    public Rigidbody rb;               // Reference to the Rigidbody
    public float mouseSensitivity = 2f; // Mouse sensitivity
    public float verticalRotationLimit = 80f; // Limit for vertical camera rotation (pitch)

    private float xRotation = 0f;  // Track the vertical rotation of the camera
    private float yRotation = 0f;  // Track the horizontal rotation of the player

    void Update()
    {
        // Horizontal (mouse X) and Vertical (mouse Y) input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Vertical rotation (camera pitch) clamped to limit
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -verticalRotationLimit, verticalRotationLimit);

        // Apply vertical rotation to the camera
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Update the horizontal rotation of the player (yaw)
        yRotation += mouseX;

        // Apply horizontal rotation to the player
        playerTransform.rotation = Quaternion.Euler(0f, yRotation, 0f);

        // Optionally, rotate the rigidbody with the camera rotation (if necessary)
        if (rb != null)
        {
            rb.rotation = Quaternion.Euler(0f, yRotation, 0f);
        }
    }
}