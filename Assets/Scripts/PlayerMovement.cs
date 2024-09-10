using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Movement on the x and z axis of player 
    [Header("Player Movement")]
    public float movementSpeed = 500;
    public Transform orientation; // Used to determine the angle movement occurs in.

    float horizontalInput;
    float verticalInput;


    Vector3 movementDirection;
    Rigidbody playerRb;


    [Header("Movement Restriction")]
    
    public float playerHeight = 2; // Used to check if player on ground
    public LayerMask playerGround; // Used to find out if player is in contact with ground
    public float playerGroundDrag = 6.5f; 
    bool grounded;

    [Header("Jumping")]
    public float jumpForce = 7.0f; // Determines the jump height
    public float jumpCooldown= 1;

    public float airMultiplier = 0.1f; // Used to alter freedom of movement when player is in the air
    public bool readyToJump; 
    public KeyCode jumpKey = KeyCode.Space; // Default key to jump is the spacebar

    public bool jumping = false;

    [Header("Sprint")]

    public KeyCode sprintKey = KeyCode.LeftShift; // Default key to toggle normal movement speed between sprint speed is  left shift
    public bool isSprinting = false; // Used to determine when to switch between the different movement speeds.

    [Header("Slope")]
    public float maxSlopeIncline;
    public RaycastHit slopeCheck;
    public int slopeDownForce; 


    public void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        playerRb.freezeRotation = true; // Prevent external forces from affecting the rotation of the object
        readyToJump = true;
    }
    private bool isOnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeCheck, playerHeight*0.5f + 0.3f))
        {
            float slopeAngle = Vector3.Angle(Vector3.up, slopeCheck.normal);
            Debug.Log("Slope Angle : " + slopeAngle);
            return (slopeAngle > 0 && slopeAngle <= maxSlopeIncline);
        }
        return false;
    }
    
    private Vector3 getSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(movementDirection, slopeCheck.normal).normalized;
    }
        
    
    void Update()
    {
        // Check if the player is on the ground or air and applying drag. 
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, playerGround);
        if (grounded)
        {
            playerRb.drag = playerGroundDrag;
        }
        else
        {
            
            playerRb.drag = 0;
        }


        // Limits player's maximum speed
        limitSpeed();


        // Get movement inputs and allows jumping
        accessInputs();

        
        if (Input.GetKeyDown(sprintKey) && !isSprinting)
        {
            sprint();
            isSprinting = true;
            Debug.Log("Left Shift");
        }


        if (Input.GetKeyUp(sprintKey))
        {
            Debug.Log("Up Left Shift");
            resetSprint();
        }



    }
   
    
    void FixedUpdate()
    {
        enforceMovement();
    }
    
    
    // Get current inputs responsible for  movement.
    void accessInputs()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal"); // A, D
        verticalInput = Input.GetAxisRaw("Vertical"); // W, S
        
        jumping = Input.GetKey(jumpKey) && readyToJump && grounded;


    }

    // Calculates and applies force based on the camera's focus.
    void enforceMovement()
    {
        // Calculation for the direction of movement
        movementDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        if (isOnSlope())
        {
            //Debug.Log("On Slope");
            playerRb.useGravity = false;
            playerRb.AddForce(-transform.up * slopeDownForce, ForceMode.Force);

        }
        else
        {
            playerRb.useGravity = true;
       
        }
        
        if (grounded)
        {
            playerRb.AddForce(movementDirection.normalized * movementSpeed * Time.deltaTime *10f, ForceMode.Force);
        }
        else
        {

            // Force is altered by a multipler when play is in the air.
            playerRb.AddForce(movementDirection.normalized * movementSpeed * Time.deltaTime * 10f * airMultiplier, ForceMode.Force);
        }


        if (jumping)
        {
            readyToJump = false;
            jump();
            // After the duration of "jumpCooldown" the "resetJump" method is called to allow the player to jump again.
            Invoke(nameof(resetJump), jumpCooldown);
        }

    }


    // Limits the maximum speed reachable by the player based on "movementSpeed"
    void limitSpeed()
    {
        Vector3 xzVelocity = new Vector3(playerRb.velocity.x, 0f,  playerRb.velocity.z);

        // Clamp the maximum movement speed.
        if(xzVelocity.magnitude > movementSpeed)
        {
            Vector3 revisedVelocity = xzVelocity.normalized * movementSpeed;
            playerRb.velocity = new Vector3(revisedVelocity.x, playerRb.velocity.y, revisedVelocity.z);

        }
    }
    



    private void jump()
    {
        // Ensure the player is not moving in the direction of the y-axis
        // NOTE: test the possibility of adding a double jump allowing y velocity.
        playerRb.velocity = new Vector3(playerRb.velocity.x, 0, playerRb.velocity.z);

        // Apply jump force in up direction of player
        playerRb.AddForce(transform.up * jumpForce, ForceMode.Impulse);


    }


    // Helper function in the cooldown of jumping
    private void resetJump()
    {
        
        readyToJump = true;
    }


    private void sprint()
    {
        // Increase movement speed to simulate sprinting
        movementSpeed = movementSpeed * 1.5f;
    }


    private void resetSprint()
    {
        isSprinting = false;
        movementSpeed = (float)(movementSpeed/ 1.5);
    }

}



