using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoostPowerup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CarController carController = other.GetComponent<CarController>();

            if (carController != null)
            {
                carController.ActivateBoost();
                Destroy(gameObject);
            }
        }
    }
}

