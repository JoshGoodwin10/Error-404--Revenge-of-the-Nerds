using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public PlayerMovement PM;
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text hpNumberText;
    public Image sliderFill;
    private float health;
    // Start is called before the first frame update
    void Start()
    {
        slider.value = health;
        hpNumberText.text = health.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        health = PM.playerHealth;
        hpNumberText.text = health.ToString();
        slider.value = health;

        sliderFill.color = Color.Lerp(Color.red, Color.green, slider.value / 100);
    }

    // SetHealth Method done by Hayley Rossouw
    public void SetHealth(float newHealth)
    {
        PM.playerHealth = (int)newHealth;
        Mathf.Clamp(newHealth, 0, 100); // Ensure health stays within bounds
        slider.value = health;
        hpNumberText.text = health.ToString();
        sliderFill.color = Color.Lerp(Color.red, Color.green, slider.value / 100);
    }
}
