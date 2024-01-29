using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private bool isBreaking;
    private float giro;
    private float pedal;

    private const float MAXBREAKFORCE = 3000F;

    [Header("Configuración motor")]
    [SerializeField] private float motorForce;
    [SerializeField] private float breakForce;

    [Header("Configuración ruedas lógicas")]
    [SerializeField] private WheelCollider FL;
    [SerializeField] private WheelCollider FR;
    [SerializeField] private WheelCollider RL;
    [SerializeField] private WheelCollider RR;

    private float currentSteerAngle;
    [Header("Configuración del giro")]
    [SerializeField] private float maxSteerAngle=30;

    [Header("Configuración de ruedas físicas")]
    [SerializeField] private Transform WFL;
    [SerializeField] private Transform WFR;
    [SerializeField] private Transform WRL;
    [SerializeField] private Transform WRR;
    [SerializeField] private bool _updateWheels;

    [Header("Velocímetro")]
    [SerializeField] private float currentSpeed;
    private const float MAXSPEED = 250f;
    private Rigidbody _rigidbody;
    public TextMeshProUGUI currentSpeedText;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        currentSpeedText = GameObject.Find("SpeedText").GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        WriteSpeedText();
    }

    private void WriteSpeedText()
    {
        float speed = currentSpeed > 0 ? currentSpeed : 0f;
        currentSpeedText.text = Mathf.Round(speed) + "Km/H";
    }

    private void FixedUpdate()
    {
        GetInput();
        HandleMotor();
        HandleSteering();
        if(_updateWheels)UpdateWheels();
    }
    
    private void GetInput()
    {
        giro = Input.GetAxis("Horizontal");
        pedal = Input.GetAxis("Vertical");

        isBreaking = Input.GetKey("space");
    }

    private void HandleMotor()
    {
        currentSpeed = _rigidbody.velocity.magnitude * 3600 / 1000;
        if (Math.Abs(currentSpeed) <MAXSPEED) 
        {
            FL.motorTorque = pedal * motorForce;
            FR.motorTorque = pedal * motorForce;
        }
        else
        {
            FL.motorTorque = 0;
            FR.motorTorque = 0;
        }

        breakForce = isBreaking ? MAXBREAKFORCE : 0f;

        FL.brakeTorque = breakForce;
        FR.brakeTorque = breakForce;
        RL.brakeTorque = breakForce;
        RR.brakeTorque = breakForce;

    }

    private void HandleSteering()
    {
        currentSteerAngle = maxSteerAngle * giro;
        FL.steerAngle = currentSteerAngle;
        FR.steerAngle = currentSteerAngle;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(FL,WFL);
        UpdateSingleWheel(FR,WFR);
        UpdateSingleWheel(RL,WRL);
        UpdateSingleWheel(RR,WRR);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 position;
        Quaternion rotation;
        
        wheelCollider.GetWorldPose(out position, out rotation);

        wheelTransform.rotation = rotation;
        wheelTransform.position = position;
    }
}
