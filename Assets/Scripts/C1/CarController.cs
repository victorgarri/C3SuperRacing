using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public static float moveSpeed = 2.0f;
    public float currentMoveSpeed = moveSpeed;
    private bool isBoostActive = false;
    private bool isBrakeActive = false;
    private bool isOilActive = false;
    private bool isTambaleando = false;

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
        else if (isOilActive)
        {
            currentMoveSpeed = moveSpeed / 1.5f;
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
    
    public void ActivateOil()
    {
        isOilActive = true;
        StartCoroutine(DeactivateBoostAfterDelay(0.0f));
        TambaleoCoroutine();
    }
    
    private IEnumerator DeactivateBoostAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isBoostActive = false;
    }
    
    private void DeactivateBrake()
    {
        isBrakeActive = false;
    }
    
    private void DeactivateOil()
    {
        isOilActive = false;
        StopTambaleo();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BrakePlane"))
        {
            ActivateBrake();
        } 
        else if (other.CompareTag("OilPlane"))
        {
            ActivateOil();
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("BrakePlane"))
        {
            DeactivateBrake();
        }
        else if (other.CompareTag("OilPlane"))
        {
            DeactivateOil();
        }
    }
    
    private void TambaleoCoroutine()
    {
        if (!isTambaleando)
        {
            StartCoroutine(Tambaleo());
        }
    }

    private IEnumerator Tambaleo()
    {
        isTambaleando = true;
        float elapsedTime = 0f;

        while (elapsedTime < 1f && isOilActive)
        {
            float xRotation = Mathf.Sin(Time.time * 10f) * 5f; // Movimiento sinusoidal
            float zRotation = Random.Range(-1f, 1f);

            transform.rotation = Quaternion.Euler(new Vector3(xRotation, 0f, zRotation));

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.rotation = Quaternion.identity;
        isTambaleando = false;
    }
    
    private void StopTambaleo()
    {
        StopCoroutine("Tambaleo");
        transform.rotation = Quaternion.identity;
        isTambaleando = false;
    }
}

