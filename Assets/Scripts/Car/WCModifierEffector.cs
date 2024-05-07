using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WCModifierEffector : MonoBehaviour
{
    private WCModifier wcModifier;

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
}