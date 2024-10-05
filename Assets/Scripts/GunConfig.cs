using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunConfig : MonoBehaviour
{

    
    [Header("Gun Stats")]
    public float damageDealt;
    public float reloadTime;
    public float fireRate;
    public float maxRange;
    public float continuousShotDelay; // a small delay between bullets fired as we are not creating a laser
    public float spread;
    public int magazineCapacity;
    public int numberMagazines;
    public bool fullAuto;
    public int bulletsPerClick;
}
