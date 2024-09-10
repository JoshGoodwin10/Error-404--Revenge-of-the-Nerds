using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health = 100;

    public float jumpingForce;
    public bool isGrounded; 
    public LayerMask ground; // Used to find out if player is in contact with ground
    public float height;
    public Rigidbody enemyRb;



    private void checkOnGround()
    {
        Debug.Log("Checking Ground");
        isGrounded = Physics.Raycast(transform.position, Vector3.down, height * 0.5f + 0.2f, ground); 
    }


    public void jump()
    {
        Debug.Log("Enemy Jump");
        enemyRb.AddForce(transform.up * jumpingForce, ForceMode.Impulse);
    }
    private void checkIsAlive() {
        if(health <= 0)
        {
            gameObject.SetActive(false);
        }

        if (isGrounded)
        {
            jump();
        }
    }

    public void takeDamage(float damage)
    {
        health -= damage;
    }



    void Update()
    {
        checkIsAlive();
        checkOnGround();
        if (isGrounded) {
            jump();
        }
    }
}
