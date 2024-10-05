using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncPlayerCamera : MonoBehaviour
{
    /*
     * Matches the position of the player's camera position (empty object) and 
     * the position of the camera that the player views the world from.
     */

    public Transform playerEyes;
    void Update()
    {
        transform.position = playerEyes.position;
    }
}
