using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrakeBoostPowerDown : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CarController carController = other.GetComponent<CarController>();

            if (carController != null)
            {
                carController.ActivateBrake();
            }
        }
    }
}
