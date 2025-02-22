using UnityEngine;

public class PlayerGroundedCheck : MonoBehaviour
{
    public bool isGrounded;
    private float groundCheckDistance;
    [SerializeField] private CapsuleCollider capsuleCollider;
    private float bufferCheckDistance = 0.2f;  // Increased buffer to prevent false negatives
    public RaycastHit slopeHit;

    private void Start()
    {
        groundCheckDistance = (capsuleCollider.height / 2) + bufferCheckDistance;
    }

    public bool CheckGroundStatus()
    {
        Vector3 rayOrigin = capsuleCollider.bounds.center; // Start ray from center of capsule
        bool rayHit = Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, groundCheckDistance);

        isGrounded = rayHit;
        return isGrounded;
    }

    public bool OnSlope()
    {
        Vector3 rayOrigin = capsuleCollider.bounds.center; // More accurate ray origin
        if (Physics.Raycast(rayOrigin, Vector3.down, out slopeHit, groundCheckDistance))
        {
            float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);

            // Only count it as a slope if the angle is significant (above 5 degrees)
            return slopeAngle > 5f;
        }
        return false;
    }
}
