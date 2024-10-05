using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingGun : MonoBehaviour
{
    private LineRenderer lr;
    private Vector3 grapplePoint;
    public LayerMask whatIsGrappleable;
    public Transform gunTip, camera, player;
    private float maxDistance = 100f;
    private SpringJoint joint;
    public float overshootYAxis = 2f;
    public bool UnReeledRope = false;

    void Awake() {
        lr = GetComponent<LineRenderer>();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Q)) {
            StartGrapple();
        }
        if (Input.GetKey(KeyCode.Q) && UnReeledRope == true) {
            //Swing();
        }
        else if(Input.GetKeyDown(KeyCode.E)){
            StopGrapple();
        }
    }

    // Called after Update
    void LateUpdate() {
	if(UnReeledRope){
           DrawRope();
        }
    }

    void StartGrapple() {
        if (!IsGrappling()) {
            RaycastHit hit;
            if (Physics.Raycast(camera.position, camera.forward, out hit, maxDistance, whatIsGrappleable)) {
                grapplePoint = hit.point;
	        UnReeledRope = true;
		joint = player.gameObject.AddComponent<SpringJoint>();
        	joint.autoConfigureConnectedAnchor = false;
        	joint.connectedAnchor = grapplePoint;
		// Adjust these values to fit your game
        	joint.spring = 4.5f;
        	joint.damper = 5f;
        	joint.massScale = 1f;
                ExecuteGrapple();
            }
            else {
		//future reference if we add a "failed grapple" animation 
                //grapplePoint = camera.position + camera.forward * maxDistance;
                //Invoke(nameof(StopGrapple), 1f);
            }
        }
    }

    void Swing() {

        float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

        // The distance grapple will try to keep from grapple point
        joint.maxDistance = distanceFromPoint * 0.8f;
        joint.minDistance = distanceFromPoint * 0.25f;

        // Adjust these values to fit your game
        joint.spring = 4.5f;
        joint.damper = 7f;
        joint.massScale = 4.5f;
    }

    void ExecuteGrapple() {
        Vector3 lowestPoint = new Vector3(player.position.x, player.position.y - 1f, player.position.z);

        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;

        if (grapplePointRelativeYPos < 0) highestPointOnArc = overshootYAxis;
	
	// Set the player's velocity to the calculated jump velocity
        player.GetComponent<Rigidbody>().velocity = CalculateJumpVelocity(player.position, grapplePoint, highestPointOnArc);

	if(player.position == grapplePoint){
           Invoke(nameof(StopGrapple), 1f);
	   
	}
    }

    void StopGrapple() {
	UnReeledRope = false;
        lr.positionCount = 0;
        Destroy(joint);
    }

    private Vector3 currentGrapplePosition;

    void DrawRope() {

        //currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 20f);
	lr.positionCount = 2;
        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, grapplePoint);
    }

    public bool IsGrappling() {
        return joint != null;
    }

    public Vector3 GetGrapplePoint() {
        return grapplePoint;
    }

    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight) {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity)
            + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }
}
