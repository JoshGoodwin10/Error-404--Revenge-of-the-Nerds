using UnityEngine;
using TMPro;

public class TokenManager : MonoBehaviour
{
    public int tokens = 10; // Starting number of tokens
    public TMP_Text tokenDisplay; // Token display

    void Start()
    {
        UpdateTokenDisplay();
    }

    public bool UseToken(int amount)
    {
        if (tokens >= amount)
        {
            tokens -= amount; // Decrease tokens
            UpdateTokenDisplay(); // Update UI display
            return true; // Token used successfully
        }
        return false; // Not enough tokens
    }

    public void AddTokens(int amount)
    {
        tokens += amount; // Method to add tokens
        UpdateTokenDisplay(); // Update UI display
    }

    private void UpdateTokenDisplay()
    {
        if (tokenDisplay != null)
            tokenDisplay.text = "Tokens Available: " + tokens; // Update the displayed tokens
    }
}