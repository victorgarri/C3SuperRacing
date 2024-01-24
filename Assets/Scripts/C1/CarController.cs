using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public static float moveSpeed = 2.0f;
    public float currentMoveSpeed = moveSpeed;
    private bool isBoostActive = false;
    private bool isBrakeActive = false;

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontalInput, 0.0f, verticalInput).normalized;

        UpdateMoveSpeed();
        MoveCar(moveDirection);
        
        Debug.Log(currentMoveSpeed);
    }
    
    private void UpdateMoveSpeed()
    {
        if (isBoostActive)
        {
            currentMoveSpeed = moveSpeed * 2.0f;
        }
        else if (isBrakeActive)
        {
            currentMoveSpeed = moveSpeed / 2.0f;
        }
        else
        {
            currentMoveSpeed = moveSpeed;
        }
    }

    private void MoveCar(Vector3 moveDirection)
    {
        transform.Translate(moveDirection * currentMoveSpeed * Time.deltaTime);
    }

    public void ActivateBoost()
    {
        isBoostActive = true;
        StartCoroutine(DeactivateBoostAfterDelay(3.0f));
    }
    
    public void ActivateBrake()
    {
        isBrakeActive = true;
        StartCoroutine(DeactivateBoostAfterDelay(0.0f));
    }
    
    private void DeactivateBrake()
    {
        isBrakeActive = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BrakePlane"))
        {
            ActivateBrake();
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("BrakePlane"))
        {
            DeactivateBrake();
        }
    }
    
    private IEnumerator DeactivateBoostAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isBoostActive = false;
    }
}

