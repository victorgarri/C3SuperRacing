using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public float moveSpeed = 2.0f;
    private bool isBoostActive = false;

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontalInput, 0.0f, verticalInput).normalized;

        MoveCar(moveDirection);
    }

    private void MoveCar(Vector3 moveDirection)
    {
        if (isBoostActive)
        {
            transform.Translate(moveDirection * (moveSpeed * 2.0f) * Time.deltaTime);
        }
        else
        {
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
        }
    }

    public void ActivateBoost()
    {
        isBoostActive = true;
        StartCoroutine(DeactivateBoostAfterDelay(3.0f));
    }

    private System.Collections.IEnumerator DeactivateBoostAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isBoostActive = false;
    }
}

