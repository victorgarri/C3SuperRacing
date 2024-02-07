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
            bounceCount = 0;
        }
        else if (collision.gameObject.CompareTag("Muro") && bounceCount < maxBounces)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            
            Vector3 normal = collision.contacts[0].normal;
            rb.AddForce(normal * 9, ForceMode.Impulse);

            bounceCount++;
            Debug.Log(bounceCount);
        }
        else
        {
            Destroy(gameObject);
            bounceCount = 0;
        }
    }
}
