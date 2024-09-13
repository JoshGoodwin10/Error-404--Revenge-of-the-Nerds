using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health = 100;


    public Rigidbody enemyRb;





 
    private void checkIsAlive() {
        if(health <= 0)
        {
            //gameObject.SetActive(false);
            Destroy(gameObject);

        }
    }

    public void takeDamage(float damage)
    {
        health -= damage;
    }



    void Update()
    {
        checkIsAlive();

    }
}
