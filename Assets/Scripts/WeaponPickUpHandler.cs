using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using TMPro.Examples;

public class WeaponPickUpHandler : MonoBehaviour
{
    [Header("Set Gun Stats")]
    GameObject gunHolder;
    GunHandler gun;
    public GunConfig weaponConfig;

    [Header("Set Visuals")]
    GameObject gunPrefab;
    public GunWithHandsReference gunWithHands;

    [Header("Pick Up Key")]
    public KeyCode pickUpKey = KeyCode.F;
    private GameObject pickUpPromptObject;
    private bool pickUp;
    private bool hasPickUpPrompt = false;
    private TextMeshProUGUI pickUpPromptTextMesh;

    private void Start()
    {

        gunHolder = GameObject.FindGameObjectWithTag("GunHolder");


        pickUpPromptObject = GameObject.FindGameObjectWithTag("PickUpPrompt");



        if (pickUpPromptObject != null)
        {
            Debug.Log("FOUND PLAYER PROMPT");
            pickUpPromptTextMesh = pickUpPromptObject.GetComponent<TextMeshProUGUI>();
            if (pickUpPromptTextMesh != null)
            {
                Debug.Log("FOUND TEXTMESHPRO");
                pickUpPromptTextMesh.text = "";
                hasPickUpPrompt = true;
            }
        }

    }
    void removeCurrentGun()
    {
        //Debug.Log("removeCurrentGun()");
        Transform parent = gunHolder.GetComponent<Transform>();

        if (parent != null)
        {


            foreach (Transform child in parent)
            {
                if (child.tag == "Gun")
                {
                    //Debug.Log("Found Child");
                    child.SetParent(null);

                    Destroy(child.gameObject);
                    //Debug.Log("Killed Child");
                }
            }
        }
    }

    void setNewGunPrefab()
    {


        if (gunWithHands != null && gunWithHands.gunWithHandsPrefab != null)
        {


            gunPrefab = gunWithHands.gunWithHandsPrefab;
            //Debug.Log("Found Gun w/ Hand Prefab.");
            GameObject newGunInstance = Instantiate(gunPrefab, transform.position, Quaternion.identity);

            if (gunHolder != null)
            {

                removeCurrentGun();
                newGunInstance.transform.SetParent(gunHolder.transform);
                newGunInstance.tag = "Gun";
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

    private void OnTriggerEnter(Collider other)
    {
        if (hasPickUpPrompt && other.gameObject.tag == "Player")
        {
            Debug.Log("In Trigger Enter");
            pickUpPromptTextMesh.text = "Press " + pickUpKey.ToString() + " to Pick Up.";

        }
    }

    void OnTriggerStay(Collider other)
    {

        accessInput();
        if (other.gameObject.tag == "Player")
        {
            if (pickUp)
            {
                pickUpWeapon();
            }
        }
    }
   
    private void OnTriggerExit(Collider other)
    {
        if (hasPickUpPrompt && other.gameObject.tag == "Player")
        {
            pickUpPromptTextMesh.text = "";

        }

    }
   
    private void pickUpWeapon()
    {
        Debug.Log("Pickup Weapon");
        gun = gunHolder.GetComponent<GunHandler>();
        setNewGunPrefab();
        gun.setGunConfig(weaponConfig);
        Destroy(gameObject); 
        if (hasPickUpPrompt)
        {
            pickUpPromptTextMesh.text = "";

        }
    }

    private void accessInput()
    {
        pickUp = Input.GetKey(pickUpKey);
    }
}
