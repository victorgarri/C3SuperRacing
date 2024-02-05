using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarController : MonoBehaviour
{
    public static float moveSpeed = 4.0f;
    public float currentMoveSpeed = moveSpeed;
    private bool isBoostActive = false;
    private bool isBrakeActive = false;
    private bool isOilActive = false;
    private bool isTambaleando = false;
    
    private bool isPowerUpCollected = false;
    public Image powerUpImage;
    public Sprite powerUpSprite;
    
    public GameObject projectilePrefab;
    public Transform spawnPoint;
    
    private void Start()
    {
        if (powerUpImage != null)
        {
            powerUpImage.gameObject.SetActive(false);
        }
    }
    
    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontalInput, 0.0f, verticalInput).normalized;

        UpdateMoveSpeed();
        MoveCar(moveDirection);
        
        Debug.Log(currentMoveSpeed);
        
        if (Input.GetKeyDown(KeyCode.Space) && isPowerUpCollected)
        {
            UsePowerUp();
            ThrowProjectile();
        }
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
        else if (other.CompareTag("PowerUp"))
        {
            CollectPowerUp();
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

    #region POWERUPS
    
    private void CollectPowerUp()
    {
        isPowerUpCollected = true;
        
        if (powerUpImage != null)
        {
            powerUpImage.sprite = powerUpSprite;
            powerUpImage.gameObject.SetActive(true);
        }
    }

    private void UsePowerUp()
    {
        if (powerUpImage != null)
        {
            powerUpImage.gameObject.SetActive(false);
        }
        
        isPowerUpCollected = false;
    }
    
    #endregion
    
    #region BOOST
    
    public void ActivateBoost()
    {
        isBoostActive = true;
        StartCoroutine(DeactivateBoostAfterDelay(3.0f));
    }
    
    private IEnumerator DeactivateBoostAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isBoostActive = false;
    }
    
    #endregion
    
    # region BRAKE
    
    public void ActivateBrake()
    {
        isBrakeActive = true;
        StartCoroutine(DeactivateBoostAfterDelay(0.0f));
    }
    
    private void DeactivateBrake()
    {
        isBrakeActive = false;
    }
    
    #endregion
    
    #region OIL
    public void ActivateOil()
    {
        isOilActive = true;
        StartCoroutine(DeactivateBoostAfterDelay(0.0f));
        ResbalarCoroutine();
    }
    
    private void DeactivateOil()
    {
        isOilActive = false;
        StopResbalar();
    }
    
    private void ResbalarCoroutine()
    {
        if (!isTambaleando)
        {
            StartCoroutine(Resbalar());
        }
    }

    private IEnumerator Resbalar()
    {
        isTambaleando = true;

        while (isOilActive)
        {
            float yRotation = Mathf.Sin(Time.time * 10f) * 5f;

            transform.rotation = Quaternion.Euler(new Vector3(0f, yRotation, 0f));

            yield return null;
        }

        transform.rotation = Quaternion.identity;
        isTambaleando = false;
    }
    
    private void StopResbalar()
    {
        StopCoroutine("Resbalar");
        transform.rotation = Quaternion.identity;
        isTambaleando = false;
    }
    
    #endregion

    #region PROJECTILE

    private void ThrowProjectile()
    {
        if (projectilePrefab != null && spawnPoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }

    #endregion
}

