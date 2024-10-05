using System.Collections;
using UnityEngine;
using TMPro;

public class AmmoPurchase : MonoBehaviour
{
    public KeyCode purchaseKey = KeyCode.X; // Key to purchase ammo
    public TMP_Text interactionPrompt; // Purchase prompt
    public int tokenCost = 100; // Cost of ammo purchase 
    public int bulletsPerPurchase = 30; // Number of bullets added per purchase
    public int magazinesPerPurchase = 5; // Number of magazines added per purchase

    private Camera playerCamera;
    private bool isLookingAtAmmoBox = false;
    private TokenManager tokenManager;
    private GunHandler gunHandler;

    void Start()
    {
        playerCamera = Camera.main;
        interactionPrompt.gameObject.SetActive(false); // Hide prompt at first
        tokenManager = FindObjectOfType<TokenManager>();
        gunHandler = FindObjectOfType<GunHandler>();
    }

    void Update()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        // Perform raycast to detect ammo station
        if (Physics.Raycast(ray, out hit, 5f))
        {
            Debug.Log(hit.collider.tag);
            // Check if the ray hit an AmmoBox 
            if (hit.collider != null && hit.collider.CompareTag("AmmoBox"))
            {
                if (!isLookingAtAmmoBox)
                {
                    interactionPrompt.gameObject.SetActive(true); // Show prompt
                    interactionPrompt.text = "X to Refill Ammo";
                    isLookingAtAmmoBox = true;
                }

                if (Input.GetKeyDown(purchaseKey)) // Check if the player presses the purchase key
                {
                    PurchaseAmmo(); // Call the purchase ammo method
                }
            }
        }
        else
        {
            if (isLookingAtAmmoBox)
            {
                interactionPrompt.gameObject.SetActive(false); // Hide prompt when looking away
                isLookingAtAmmoBox = false;
            }
        }
    }

    // Method to purchase ammo
    void PurchaseAmmo()
    {
        // Check if adding bullets exceeds maximum capacity
        if (gunHandler.CanAddBullets(bulletsPerPurchase))
        {
            if (tokenManager.UseToken(tokenCost))
            {
                // Add ammo to the GunHandler's ammo system
                gunHandler.AddBullets(bulletsPerPurchase, magazinesPerPurchase);
                Debug.Log("Ammo purchased! Bullets added: " + bulletsPerPurchase);
            }
            else
            {
                Debug.Log("Not enough tokens to purchase ammo.");
            }
        }
        else
        {
            Debug.Log("Cannot purchase ammo. Max capacity reached.");
        }

        interactionPrompt.gameObject.SetActive(false); // Hide prompt after purchase
    }
}


   