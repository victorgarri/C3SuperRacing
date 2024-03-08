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
    private float cameraTimestamp=0;
    

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

    [SerializeField] private float radius = 1;
    [SerializeField] private float wheelBase;
    [SerializeField] private float trackWidth;
    [SerializeField] private bool antiAckerman = false;
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
        _rigidbody.centerOfMass = new Vector3(0, -.23f, 0.1f);
        _playerInput = GetComponent<PlayerInput>();
        _cameraPivot = GameObject.Find("CameraPivot");
        
        if(isLocalPlayer)
            transform.Find("CameraPivot/Camera").gameObject.SetActive(true);
        
        //Pillo la aguja al inicio del juego
        agujaVelocimetro = GameObject.Find("ImagenAguja").transform;
        if (agujaVelocimetro != null)
        {
            Debug.Log("Objeto registrado");
        }
        else
        {
            Debug.Log("No he encontrado nada");
        }
        
        wheelBase = Mathf.Abs(FL.transform.position.z - RL.transform.position.z);
        trackWidth = Mathf.Abs(FR.transform.position.x - FL.transform.position.x);
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
        Debug.Log(velocidad);
        if (velocidad > 0)
        {
            float velocidadNormal = velocidad / VELOCIDADMAXIMA;

            agujaVelocimetro.localEulerAngles = new Vector3(0, 0, 
                Mathf.Lerp(LIMITEANGULOIZQUIERDO, LIMITEANGULODERECHO, velocidadNormal));   
        }

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
        if (cameraInput == 0)
        {
            if (cameraTimestamp == 0) cameraTimestamp = Time.time;
            else if(Time.time-cameraTimestamp>=2)cameraOffset *= .99f;
        }
        else
        {
            cameraTimestamp = 0;
        }
        cameraOffset = cameraOffset + cameraInput * cameraSpeed;
        if (cameraOffset > 180) cameraOffset -= 360;
        else if (cameraOffset < -180) cameraOffset += 360;

        
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
        velocidad = _rigidbody.velocity.magnitude * 3600 / 1000;
        if (Math.Abs(velocidad) < VELOCIDADMAXIMA) 
        {
            FL.motorTorque = motorForce/2*pedal;
            FR.motorTorque = motorForce/2*pedal;
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
        // currentSteerAngle = maxSteerAngle * giro;
        // FL.steerAngle = currentSteerAngle;
        // FR.steerAngle = currentSteerAngle;
        if (giro > 0)
        {
            if (!antiAckerman)
            {
                FL.steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (radius + (trackWidth / 2))) * giro;
                FR.steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (radius - (trackWidth / 2))) * giro;   
            }
            else
            {
                FL.steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (radius - (trackWidth / 2))) * giro;
                FR.steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (radius + (trackWidth / 2))) * giro;
            }
        } else if (giro < 0)
        {
            if (!antiAckerman)
            {
                FL.steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (radius - (trackWidth / 2))) * giro;
                FR.steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (radius + (trackWidth / 2))) * giro;
            }
            else
            {
                FL.steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (radius + (trackWidth / 2))) * giro;
                FR.steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (radius - (trackWidth / 2))) * giro;
            }
        } else {
            FL.steerAngle = 0;
            FR.steerAngle = 0;
        }
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
