using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class WeaponPickUpHandler : MonoBehaviour
{

    [Header("Set Gun Stats")]
    GameObject gunHolder;
    GunHandler gun;
    public GunConfig weaponConfig;

    [Header("Set Visuals")]
    GameObject gunPrefab;
    public GunWithHandsReference gunWithHands;



    private void Start()
    {
        gunHolder = GameObject.FindGameObjectWithTag("GunHolder");
        Debug.Log("WeaponPickUpHandler : GunHolder -> " + gunHolder.name);
    }
    void removeCurrentGun()
    {
        Debug.Log("removeCurrentGun()");
        Transform parent = gunHolder.GetComponent<Transform>();

        if (parent != null)
        {


            foreach (Transform child in parent)
            {
                if (child.tag == "Gun")
                {
                    Debug.Log("Found Child");
                    child.SetParent(null);

                    Destroy(child.gameObject); 
                    Debug.Log("Killed Child");
                }
            }
        }
    }

    void setNewGunPrefab()
    {


        if (gunWithHands != null && gunWithHands.gunWithHandsPrefab != null)
        {

            
            gunPrefab = gunWithHands.gunWithHandsPrefab;
            Debug.Log("Found Gun w/ Hand Prefab.");
            // Instantiate the prefab at the desired position and rotation
            GameObject newGunInstance = Instantiate(gunPrefab, transform.position, Quaternion.identity);

            // Optionally, set the parent of the instantiated prefab
            if (gunHolder != null)
            {
                
                removeCurrentGun();
                newGunInstance.transform.SetParent(gunHolder.transform);
                newGunInstance.tag = "Gun";
                // Optionally adjust local position, rotation, and scale if needed
                newGunInstance.transform.localPosition = new Vector3(-1.52f, -0.3f, -1.07f); ;
                newGunInstance.transform.localRotation = Quaternion.identity;
                newGunInstance.transform.localScale = new Vector3(4, 4, 1);
            }
        }
        else
        {
            Debug.LogError("NewGunPrefab is not assigned!");
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            gun = gunHolder.GetComponent<GunHandler>();
            setNewGunPrefab();
            gun.setGunConfig(weaponConfig);
            Debug.Log("Weapon Change Config");
            Destroy(gameObject);
        }



    }

}
