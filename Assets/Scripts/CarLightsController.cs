using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarLightsController : MonoBehaviour
{
    
    [SerializeField]
    private bool carLights=false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.gameObject.CompareTag("Player"))
        {
            other.transform.parent.gameObject.GetComponent<CarController>().SetCarLights(carLights);
        }
    }
}
