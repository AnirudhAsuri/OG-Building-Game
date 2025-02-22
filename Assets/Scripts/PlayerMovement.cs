using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Vector3 inputVector;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float sprintMultiplier = 1.5f; // Multiplier for sprinting speed
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private PlayerGroundedCheck playerGroundedCheck;
    public Vector3 moveDirection;
    private Vector3 slopeMoveDir;
    public bool isGrounded;

    private void Update()
    {
        // Update slope move direction and check if the player is grounded
        slopeMoveDir = Vector3.ProjectOnPlane(moveDirection, playerGroundedCheck.slopeHit.normal);
        isGrounded = playerGroundedCheck.CheckGroundStatus();

        // Get player input for movement and jumping
        GetMovementInputs();
    }

    public void GetMovementInputs()
    {
        inputVector = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
    }

    public void MovePlayer()
    {
        moveDirection = transform.TransformDirection(inputVector) * moveSpeed;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveDirection *= sprintMultiplier;
        }

        if (isGrounded && !playerGroundedCheck.OnSlope())
        {
            rb.velocity = new Vector3(moveDirection.x, rb.velocity.y, moveDirection.z);
        }
        else if (isGrounded && playerGroundedCheck.OnSlope())
        {
            rb.velocity = new Vector3(slopeMoveDir.x, rb.velocity.y, slopeMoveDir.z);
        }
    }

    public void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    public void HandleJump()
    {
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            Jump();
        }
    }
}
