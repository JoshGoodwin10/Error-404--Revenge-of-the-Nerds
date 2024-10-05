using UnityEngine;


public class PlayerCamera : MonoBehaviour
{
    // Controls Mouse-Look sensitivity
    public float senX; 
    public float senY;



    
    public Transform orientation; // Orientation of the player - used for rotation on the y-axis 


    float xRotation;
    float yRotation;


    void Start()
    {
        // Hides the cursor from the viewer
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    void Update()
    {


       
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * senX; // Gets left-right mouse movement
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * senY; // Gets up-down mouse movement
        // Note: Time.deltaTime is used to make sure the frame rate does affect move movement value obtained.
        
        
        yRotation += mouseX; // Change for left-right look
        xRotation -= mouseY; // Change for up-down look


        // Limit up-down look range to 180 degree 
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);


       
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0); 
        
        // Change Player rotation on the y-axis
        orientation.rotation = Quaternion.Euler(0, yRotation, 0); // Body Rotation


    }
}
