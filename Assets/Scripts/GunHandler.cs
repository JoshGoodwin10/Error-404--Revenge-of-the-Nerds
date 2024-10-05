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
    public int numberMagazines;
    public bool fullAuto;
    public int bulletsPerClick; // Used for shotgun style weapons
    public int remainingBullets; // Remaining bullets that can be fired before a reload


    // Josh's Additions, to deal with UI
    [SerializeField] public TMP_Text ammoText;


    // TODO : Add option to count each bullet as a bullet or multiple bullets as 1 bullet.

    int bulletsShot; // Used to count the amount of bullets remaining to be fired per click
    public bool shooting, readyToShoot, reloading; // Used for state checks.




    [Header("Projectile")]
    [SerializeField] private GameObject bulletPrefab;  
    [SerializeField] private Transform muzzleObject; 


    [Header("Gun View")]
    public Camera gunCamera;
    public RaycastHit bulletLine;
    public Transform reticlePosition;
  
    
    public LayerMask targetHit;


    [Header("Graphics")]
    public GameObject bulletHole;
    
    public Transform muzzleTransform;
    public GameObject muzzlePrefab;


    void Update()
    {
        accessInput();

        // Josh, to update the UI
        if (reloading)
        {
            ammoText.text = "Reloading...";
        }
        
        else 
        {
            ammoText.text = remainingBullets + " | " + numberMagazines;
        }
    }
    void Start()
    {
        //Debug.Log("Start Gun Handler");
        remainingBullets = magazineCapacity;
        readyToShoot = true;
        

        // TODO : maybe move to update
        // Josh's addition, to set inital UI bullet count
        //ammoText.text = remainingBullets + " | " + magazineCapacity;
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



        if (Input.GetKeyDown(KeyCode.R) && remainingBullets < magazineCapacity && numberMagazines > 0 && !reloading)
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

        yield return new WaitForSeconds(reloadTime);
        ammoText.text = remainingBullets + " | " + numberMagazines;

        remainingBullets = magazineCapacity;
        numberMagazines--;
        reloading = false;
    }

    // Done by Hayley Rossouw
    public void AddBullets(float amount, float magazines)
    { 
        remainingBullets = Mathf.Min(remainingBullets + Mathf.FloorToInt(amount), magazineCapacity);
        numberMagazines = Mathf.Min(magazineCapacity + Mathf.FloorToInt(magazines), 5);
    }

    public void fire()
    {
        Vector3 spreadModification = new Vector3(Random.Range(-spread, spread), Random.Range(-spread, spread), 0);
        //Debug.Log(spreadModification);
        readyToShoot = false;



        bulletsShot += 1;

        //// Josh, to update the UI
        //ammoText.text = remainingBullets + " | " + magazineCapacity;

        Vector3 rawDirection = gunCamera.transform.forward; // Normal Reticle Position
        Vector3 bulletLineDirection = rawDirection + spreadModification;

        //Debug.Log("Raw Aim : " + rawDirection);
        //Debug.Log("Bullet Direction : " + bulletLineDirection);

        if (muzzlePrefab != null && muzzleTransform != null)
        {
            Instantiate(muzzlePrefab, muzzleTransform.position, muzzleTransform.rotation);
            //Debug.Log("Muzzle flash instantiated at position: " + muzzleTransform.position);
        }

        // Shoot from the view of the gunCamera
        if (Physics.Raycast(gunCamera.transform.position, bulletLineDirection, out bulletLine, targetHit))
        {
            //Debug.Log("PAIN");
            GameObject hitObject = bulletLine.collider.gameObject;
            //Debug.Log("Bullet Line Hit -> " + hitObject.name);
            //Debug.Log("Bullet Line Hit Layer -> " + hitObject.layer);

            if (bulletLine.collider.CompareTag("Enemy"))
            {
                Enemy enemy = hitObject.GetComponent<Enemy>();
                
                if (enemy != null)
                {
                    enemy.takeDamage(damageDealt);
                }
            }
            else
            {
                //Debug.Log("No Enemy Hit");
            }

        }

        // Graphics
        //Instantiate(bulletHole, bulletLine.point, Quaternion.Euler(0, 180, 0));
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

    private void FireProjectile()
    {
        readyToShoot = false;
        Debug.Log("Firing projectile...");

        // Calculate aim point and direction
        Vector3 aimPoint = gunCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, gunCamera.farClipPlane));
        Vector3 rawDirection = (aimPoint - muzzleObject.position).normalized;
        Vector3 spreadOffset = new Vector3(Random.Range(-spread, spread), Random.Range(-spread, spread), 0);
        Vector3 bulletDirection = rawDirection + spreadOffset;

        // Log details about the aim point and direction
        //Debug.Log("Aim Point: " + aimPoint + ", Raw Direction: " + rawDirection + ", Spread Offset: " + spreadOffset + ", Bullet Direction: " + bulletDirection);

        // Instantiate the bullet
        GameObject bullet = Instantiate(bulletPrefab, muzzleObject.position, Quaternion.LookRotation(bulletDirection));
        //Debug.Log("Bullet instantiated at: " + muzzleObject.position + ", Direction: " + bulletDirection);

        // Log bullet properties if the bullet prefab has a Rigidbody component
        Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
        if (bulletRigidbody != null)
        {
            // Assuming the bullet prefab moves based on its Rigidbody settings (e.g., forces or other logic)
            //Debug.Log("Bullet Rigidbody found.");
        }
        else
        {
            //Debug.LogWarning("Bullet Rigidbody not found on bullet prefab.");
        }

        // Instantiate muzzle flash effect
        if (muzzlePrefab != null && muzzleTransform != null)
        {
            Instantiate(muzzlePrefab, muzzleTransform.position, muzzleTransform.rotation);
            //Debug.Log("Muzzle flash instantiated.");
        }

        // Allow shooting again after the fire rate delay
        Invoke("resetShot", fireRate);
    }

    // Done by Hayley Rossouw
    public bool CanAddBullets(int amount)
    {
        // Check if the current bullets are less than the magazine capacity
        if (remainingBullets < magazineCapacity)
        {
            // Calculate how many bullets can be added without exceeding max capacity
            int neededBullets = magazineCapacity - remainingBullets;

            // If needed bullets are less than the requested amount, allow purchase
            if (neededBullets < amount)
            { 
                return true;
            }

            return true;

        }

        // If already at maximum capacity, return false
        return false;
    }

    public void setGunConfig(GunConfig gun)
    {
        //Debug.Log("TEST GUN CONFIG");
        this.damageDealt = gun.damageDealt;
        this.reloadTime = gun.reloadTime;
        this.fireRate = gun.fireRate;

        //Debug.Log("gun fire rate : " + (float)(gun.fireRate));
        this.maxRange = gun.maxRange;
        this.continuousShotDelay = gun.continuousShotDelay;
        this.spread = gun.spread;
        this.magazineCapacity = gun.magazineCapacity;
        this.numberMagazines = gun.numberMagazines;
        this.fullAuto = gun.fullAuto;
        this.bulletsPerClick = gun.bulletsPerClick;
        this.remainingBullets = gun.magazineCapacity;
    }
}
