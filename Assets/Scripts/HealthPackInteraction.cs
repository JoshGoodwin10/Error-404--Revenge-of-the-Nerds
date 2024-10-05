using UnityEngine;
using TMPro;

public class HealthPackInteraction : MonoBehaviour
{
    public KeyCode useKey = KeyCode.X; // Key to use the health pack
    public TMP_Text interactionPrompt; // Show the message
    public int tokenCost = 1; // Cost health pack

    private Camera playerCamera;
    private bool isLookingAtHealthPack = false;
    private TokenManager tokenManager;

    void Start()
    {
        playerCamera = Camera.main;
        interactionPrompt.gameObject.SetActive(false); // Hide prompt at first
        tokenManager = FindObjectOfType<TokenManager>(); 
    }

    void Update()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 5f)) // Raycast to detect health pack
        {
            if (hit.collider != null && hit.collider.CompareTag("HealthPack")) // Check for HealthPack tag
            {
                if (!isLookingAtHealthPack)
                {
                    interactionPrompt.gameObject.SetActive(true); // Show prompt
                    interactionPrompt.text = "X to Purchase Health";
                    isLookingAtHealthPack = true;
                }

                if (Input.GetKeyDown(useKey)) // Check for key press
                {
                    UseHealthPack();
                }
            }
        }
        else
        {
            if (isLookingAtHealthPack)
            {
                interactionPrompt.gameObject.SetActive(false); // Hide prompt
                isLookingAtHealthPack = false;
            }
        }
    }

    void UseHealthPack()
    {
        if (tokenManager.UseToken(tokenCost)) // Check if token can be used
        {
            HealthBar healthBar = FindObjectOfType<HealthBar>();
            healthBar.SetHealth(100); // Set health to full
            Debug.Log("Health pack used! Health restored to full.");
        }
        else
        {
            Debug.Log("Not enough tokens to use health pack!");
        }

        interactionPrompt.gameObject.SetActive(false); // Hide prompt after using
    }
}
