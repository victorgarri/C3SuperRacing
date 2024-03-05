using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


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
    [SerializeField] private float maxSteerAngle=30;

    [Header("Configuración de ruedas físicas")]
    [SerializeField] private Transform WFL;
    [SerializeField] private Transform WFR;
    [SerializeField] private Transform WRL;
    [SerializeField] private Transform WRR;
    [SerializeField] private bool _updateWheels;

    [Header("Velocímetro")]
    private const float LIMITEANGULOIZQUIERDO = 190f;
    private const float LIMITEANGULODERECHO = -100f;
    public Transform agujaVelocimetro;
    private const float VELOCIDADMAXIMA = 80f;
    private float velocidad = 0f;
    
    
    private Rigidbody _rigidbody;
    private PlayerInput _playerInput;
    private GameObject _cameraPivot;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.centerOfMass = new Vector3(0, -.2f, 0);
        // Debug.Log("Start: "+isLocalPlayer);
        _playerInput = GetComponent<PlayerInput>();
        _cameraPivot = GameObject.Find("CameraPivot");
        if (isLocalPlayer)
        {
            transform.Find("CameraPivot/Camera").gameObject.SetActive(true);
        }
        
        //Pillo la aguja al inicio del juego
        agujaVelocimetro = GameObject.Find("ImagenAguja").transform;

    }
    

    private void Update()
    {
        if (isLocalPlayer)
        {
            ActualizacionAgujaVelocimetro();
        }

        _cameraPivot.transform.position = transform.position;
        _cameraPivot.transform.rotation = Quaternion.Euler(0,this.transform.eulerAngles.y + (cameraOffset),0);
        
    }

    private void ActualizacionAgujaVelocimetro()
    {
        //Debug.Log(velocidad);
        float velocidadNormal = velocidad / VELOCIDADMAXIMA;

        agujaVelocimetro.localEulerAngles = new Vector3(0, 0, 
            Mathf.Lerp(LIMITEANGULOIZQUIERDO, LIMITEANGULODERECHO, velocidadNormal));   

    }

    private void FixedUpdate()
    {
        Debug.Log("Update: "+isLocalPlayer);
        
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
        
        Debug.Log("Camera offset"+cameraOffset);
    }

    private void GetInput()
    {
        // giro = Input.GetAxis("Horizontal");
        giro = _playerInput.actions["Steer"].ReadValue<float>();

        // pedal = Input.GetAxis("Vertical");
        pedal = _playerInput.actions["Throtle"].ReadValue<float>();

        cameraInput = _playerInput.actions["Camera"].ReadValue<float>();
        Debug.Log("Camera input: "+cameraInput);

        isBreaking = _playerInput.actions["Brake"].IsPressed();
    }

    private void HandleMotor()
    {
        velocidad = _rigidbody.velocity.magnitude * 3600 / 1000;
        if (Math.Abs(velocidad) < VELOCIDADMAXIMA) 
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
