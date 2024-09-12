using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class GunHandler : MonoBehaviour
{


    [Header("Gun Statistics")]
    public float damageDealt;
    public float reloadTime;
    public float fireRate;
    public float maxRange;
    public float continuousShotDelay; // a small delay between bullets fired as we are not creating a laser
    public float spread;
    public int magazineCapacity;
    public bool fullAuto;
    public int bulletsPerClick; // Used for shotgun style weapons
    public int remainingBullets; // Remaining bullets that can be fired before a reload

    // Josh's Additions, to deal with UI
    [SerializeField] public TMP_Text ammoText;


    // TODO : Add option to count each bullet as a bullet or multiple bullets as 1 bullet.

    int bulletsShot; // Used to count the amount of bullets remaining to be fired per click
    public bool shooting, readyToShoot, reloading; // Used for state checks.







    [Header("Gun View")]
    public Camera gunCamera;
    public RaycastHit bulletLine;
    public Transform reticlePosition;
    public LayerMask targetHit;


    [Header("Graphics")]
    public GameObject bulletHole;
    //public Transform muzzleObject;
    //public GameObject muzzleFlash;


    void Update()
    {
        accessInput();
    }
    void Start()
    {
        //Debug.Log("Start Gun Handler");
        remainingBullets = magazineCapacity;
        readyToShoot = true;
        // Josh's addition, to set inital UI bullet count
        ammoText.text = remainingBullets + " | " + magazineCapacity;
    }
    public void accessInput()
    {
        // Check if hold down fire is allowed

        if (fullAuto)
        {
            shooting = Input.GetKey(KeyCode.Mouse0);
        }
        else
        {
            shooting = Input.GetKeyDown(KeyCode.Mouse0);
        }



        if (Input.GetKeyDown(KeyCode.R) && remainingBullets < magazineCapacity && !reloading)
        {
            StartCoroutine(reload());   // Changed by Josh to use a timer
        }
        if (shooting && readyToShoot && !reloading && remainingBullets > 0)
        {
            bulletsShot = 0;
            fire();
        }

    }

    IEnumerator reload()
    {
        reloading = true;
        ammoText.text = "Reloading...";

        yield return new WaitForSeconds(reloadTime);
        ammoText.text = remainingBullets + " | " + magazineCapacity;

        Invoke("completeReload", reloadTime);
    }


    void completeReload()
    {
        remainingBullets = magazineCapacity;
        reloading = false;
    }

    public void fire()
    {
        Vector3 spreadModification = new Vector3(Random.Range(-spread, spread), Random.Range(-spread, spread), 0);
        Debug.Log(spreadModification);
        readyToShoot = false;



        bulletsShot += 1;

        // Josh, to update the UI
        ammoText.text = remainingBullets + " | " + magazineCapacity;

        Vector3 rawDirection = gunCamera.transform.forward; // Normal Reticle Position
        Vector3 bulletLineDirection = rawDirection + spreadModification;

       // Debug.Log("Raw Aim : " + rawDirection);
        //Debug.Log("Bullet Direction : " + bulletLineDirection);


        // Shoot from the view of the gunCamera
        if (Physics.Raycast(gunCamera.transform.position, bulletLineDirection, out bulletLine, targetHit))
        {
            GameObject hitObject = bulletLine.collider.gameObject;
            Debug.Log(hitObject.name + " test");
            if (bulletLine.collider.CompareTag("Enemy"))
            {
                Debug.Log("Enemy Hit");
                Enemy enemy = hitObject.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.takeDamage(damageDealt);
                }
            }
            else
            {
                Debug.Log("No Enemy Hit");
            }

        }

        // Graphics
        Instantiate(bulletHole, bulletLine.point, Quaternion.Euler(0, 180, 0));
        //Instantiate(muzzleFlash, muzzleObject.transform.position, Quaternion.identity);

        // Handles multiple shots from one click
        if (remainingBullets > 0 && bulletsShot < bulletsPerClick)
        {
            //Debug.Log("Continuing Firing remaining shots.");
            Invoke("fire", continuousShotDelay);
        }
        else
        {
            remainingBullets -= 1;
        }

        Invoke("allowShooting", fireRate);
        
        
    }

    void allowShooting()
    {
        readyToShoot = true;
    }
}
