using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WCModifier : MonoBehaviour
{
    [SerializeField] private float boostStrength;
    public void ApplyEffect(GameObject wheel)
    {
        Rigidbody carRigidBody = wheel.transform.parent.GetComponent<Rigidbody>();
        
        carRigidBody.AddForce(carRigidBody.transform.forward * boostStrength,ForceMode.Force);
    }
}
