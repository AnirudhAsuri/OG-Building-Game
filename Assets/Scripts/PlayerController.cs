using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerGroundedCheck playerGroundedCheck;


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        playerMovement.GetMovementInputs();
    }

    private void FixedUpdate()
    { 
        playerMovement.MovePlayer();
        playerMovement.HandleJump();
    }
}
