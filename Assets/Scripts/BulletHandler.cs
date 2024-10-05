using UnityEngine;

public class BulletHandler : MonoBehaviour
{
    public float speed = 10000f;
    public float damage = 10f;
    public LayerMask targetHit; // Layer of the objects that can be hit
    public float lifespan = 3f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * speed); // Move bullet forward

        // Destroy the bullet after lifespan
        Destroy(gameObject, lifespan);
    }

    void OnCollisionEnter(Collision collision)
    {
        GameObject hitObject = collision.gameObject;

        // Destroy the bullet on collision
        Destroy(gameObject);
    }
}
