using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text hpNumberText;
    public Image sliderFill;
    private float health = 100;
    // Start is called before the first frame update
    void Start()
    {
        slider.value = health;
        hpNumberText.text = health.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Q))
        {
            if (health > 0)
            {
                health -= 5f;
                slider.value = health;
                hpNumberText.text = health.ToString();
            }
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            if (health <= 99)
            {
                health += 5f;
                slider.value = health;
                hpNumberText.text = health.ToString();
            }
        }

        sliderFill.color = Color.Lerp(Color.red, Color.green, slider.value / 100);
    }
}
