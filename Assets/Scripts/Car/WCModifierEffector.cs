using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WCModifierEffector : MonoBehaviour
{
    [SerializeField] private WheelCollider _wheelCollider;

    

    // Update is called once per frame
    void Update()
    {
        if (_wheelCollider)
        {
            WheelHit hit;
            _wheelCollider.GetGroundHit(out hit);
            if (hit.collider && hit.collider.gameObject.CompareTag("WCModifier"))
            {
                hit.collider.gameObject.GetComponent<WCModifier>().ApplyEffect(gameObject);
            }
        }
    }
}
