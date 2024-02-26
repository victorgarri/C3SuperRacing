using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;


public class CarController : NetworkBehaviour
{
    private bool isBreaking;
    private float giro;
    private float pedal;
    private float cameraInput;
    private float cameraSpeed=4.5f;
    private float cameraOffset=0;
    

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
    [SerializeField] private float maxSteerAngle=20;

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

    private PlayerInput _playerInput;
    private GameObject _cameraPivot;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        currentSpeedText = GameObject.Find("TextoVelocimetro").GetComponent<TextMeshProUGUI>();
        _rigidbody.centerOfMass = new Vector3(0, -.2f, .2f);
        // Debug.Log("Start: "+isLocalPlayer);
        _playerInput = GetComponent<PlayerInput>();
        _cameraPivot = GameObject.Find("CameraPivot");
        if(isLocalPlayer)
            transform.Find("CameraPivot/Camera").gameObject.SetActive(true);
        
    }
    

    private void Update()
    {
        if (isLocalPlayer)
        {
            WriteSpeedText();
        }

        _cameraPivot.transform.position = transform.position;
        _cameraPivot.transform.rotation = Quaternion.Euler(0,this.transform.eulerAngles.y + (cameraOffset),0);
        
    }

    private void WriteSpeedText()
    {
        float speed = currentSpeed > 0 ? currentSpeed : 0f;
        currentSpeedText.text =Mathf.Round(speed).ToString();
    }

    private void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            GetInput();
            HandleMotor();
            HandleSteering();
            HandleCamera();
        }
        if(_updateWheels)UpdateWheels();
    }

    private void HandleCamera()
    {
        cameraOffset = cameraOffset + cameraInput * cameraSpeed;
        if (cameraOffset > 180) cameraOffset -= 360;
        else if (cameraOffset < -180) cameraOffset += 360;
        if (cameraInput == 0) cameraOffset *= 1 - Math.Abs(pedal) * 0.1f;
    }

    private void GetInput()
    {
        // giro = Input.GetAxis("Horizontal");
        giro = _playerInput.actions["Steer"].ReadValue<float>();

        // pedal = Input.GetAxis("Vertical");
        pedal = _playerInput.actions["Throtle"].ReadValue<float>();

        cameraInput = _playerInput.actions["Camera"].ReadValue<float>();

        isBreaking = _playerInput.actions["Brake"].IsPressed();
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
