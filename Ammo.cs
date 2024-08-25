using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Ammo : MonoBehaviour
{
    private float maxCapacity;
    private float bulletsRemaining;
    private float roundsLeft;
     private bool isReloading = false;
    private float reloadDelay = 1f;

    [SerializeField] public TMP_Text ammoText;
    // Start is called before the first frame update
    void Start()
    {
        roundsLeft = 5f;
        maxCapacity = 20f;
        bulletsRemaining = maxCapacity;
        ammoText.text = roundsLeft + " | " + bulletsRemaining;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S) && bulletsRemaining > 0)
        {
            bulletsRemaining--;
            ammoText.text = roundsLeft + " | " + bulletsRemaining;
            
            if (bulletsRemaining == 0 && roundsLeft > 0)
            {
                StartCoroutine(Reload());
            }
        }

        if (Input.GetKeyUp(KeyCode.R) && roundsLeft > 0)
        {
            StartCoroutine(Reload());
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        ammoText.text = "Reloading...";

        yield return new WaitForSeconds(reloadDelay);

        roundsLeft--;
        bulletsRemaining = maxCapacity;
        ammoText.text = roundsLeft + " | " + bulletsRemaining;

        isReloading = false;
    }
}
