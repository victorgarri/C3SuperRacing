using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovPowerUps : MonoBehaviour
{
    private Rigidbody body;
    private float horizontalInput;
    private float rotationSpeed = 0.6f;
    
    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        this.transform.Rotate(new Vector3 (0, -180, 0)*rotationSpeed*Time.deltaTime);
    }

}
