using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class WCModifierEffector : NetworkBehaviour
{
    private WCModifier wcModifier;
    public float brakeStrength;

    private void Start()
    {
        wcModifier = FindObjectOfType<WCModifier>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("WCModifier"))
        {
            wcModifier.ApplyEffect(gameObject);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Proyectil"))
        {
            ReduceSpeed(gameObject);
        }
    }

    public void ReduceSpeed(GameObject car)
    {
        Rigidbody carRigidBody = car.GetComponent<Rigidbody>();

        carRigidBody.velocity *= brakeStrength;

        CrearCorrutinaLuces();
    }

    [Command]
    private void CrearCorrutinaLuces()
    {
        StartCoroutine(ParpadeoLucesCoroutine(3));
    }
    
    private IEnumerator ParpadeoLucesCoroutine(float seconds)
    {
        float duration = 3f;
        float interval = 0.2f;
        float timer = 0f;
        bool activate = false;
        
        while (timer < duration)
        {
            GetComponent<CarController>().SetCarLights(activate);
            activate = !activate;
            timer += interval;
            
            yield return new WaitForSeconds(interval);
        }
        GetComponent<CarController>().SetCarLights(true);
    }

}