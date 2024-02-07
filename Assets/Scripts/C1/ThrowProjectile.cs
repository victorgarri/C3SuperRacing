using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowProjectile : MonoBehaviour
{
    private GameObject player;
    private Collider projectileCollider;
    public int bounceCount = 0;
    private const int maxBounces = 3;
    
    public void SetPlayer(GameObject playerObject)
    {
        player = playerObject;
    }
    
    void Start()
    {
        projectileCollider = GetComponent<Collider>();
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
        else
        {
            if (bounceCount < maxBounces)
            {
                Rigidbody rb = GetComponent<Rigidbody>();
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, -rb.velocity.z);

                bounceCount++;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
