using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ItemPickUpHandler : MonoBehaviour
{


// Handle Player Stat Change

    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Pick Up Collision");
        }
        if (other.gameObject.tag == "Player" && gameObject.tag == "reloadPickUp")
        {
            Debug.Log("Set Ammo Mag Full");
        }






    }

}
