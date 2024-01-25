using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Mirror;

public class CarControllerArcade : NetworkBehaviour 
{
    private TextMeshProUGUI currentSpeedText;
    public Rigidbody RBSphere;

    public float reverseSpeed = 100;
    public float turnSpeed = 150;
    public float airDrag = .1f;
    public float groundDrag = 4f;
    public float torque;
    private bool isCarGrounded;
    public LayerMask groundLayer;
    
    private bool isBreaking;
    private float giro;
    private float pedal;

    private const float MAXBREAKFORCE = 3000F;

    [Header("Configuración motor")]
    [SerializeField] private float motorForce=250;
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

    [Header("Velocímetro")]
    [SerializeField] private float currentSpeed;
    private const float MAXSPEED = 250f;
    private Rigidbody _rigidbody;
    [SerializeField] private GameObject _playerCamera;
    

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        RBSphere.transform.parent = null;
        if (isLocalPlayer)
        {
            _playerCamera.SetActive(true);
        }

        currentSpeedText = GameObject.Find("SpeedText").GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (isLocalPlayer)
        {
            WriteSpeedText();
        }
    }

    private void WriteSpeedText()
    {
        float speed = currentSpeed > 0 ? currentSpeed : 0f;
        currentSpeedText.text = Mathf.Round(speed) + "Km/H";
    }

    private void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            GetInput();
            HandleMotor();
            HandleSteering();
            UpdateWheels();
        }
    }
    
    private void GetInput()
    {
        giro = Input.GetAxis("Horizontal");
        pedal = Input.GetAxis("Vertical");

        isBreaking = Input.GetKey("space");
    }

    private void HandleMotor()
    {
        torque = pedal > 0 ? pedal * motorForce : pedal * reverseSpeed;
        
        transform.position = RBSphere.transform.position;

        RaycastHit hit;
        isCarGrounded = Physics.Raycast(transform.position, -transform.up, out hit, 5.1f, groundLayer);
        
        transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;

        if (isCarGrounded)
        {
            RBSphere.drag = groundDrag;
            RBSphere.AddForce(transform.forward * torque, ForceMode.Acceleration);
        }
        else
        {
            RBSphere.drag = airDrag;
            RBSphere.AddForce(transform.up * -30);
        }

        currentSpeed = RBSphere.velocity.magnitude * 3600 / 1000;


        /*currentSpeed = _rigidbody.velocity.magnitude * 3600 / 1000;
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
        RR.brakeTorque = breakForce;*/

    }

    private void HandleSteering()
    {
        // Giro del coche
        float newRotation = giro * turnSpeed * Time.deltaTime * pedal;
        transform.Rotate(0, newRotation,0, Space.World);

        /*currentSteerAngle = maxSteerAngle * giro;
        FL.steerAngle = currentSteerAngle;
        FR.steerAngle = currentSteerAngle;*/
    }

    private void UpdateWheels()
    {
        // Giro fisico de las ruedas, solo es visual

        var anguloGiro = giro * maxSteerAngle;
        // WFL.localEulerAngles = new Vector3(0, anguloGiro, 0);
        // WFR.localEulerAngles = new Vector3(0, anguloGiro, 0);
        //
        // WFL.transform.Rotate(torque,0f,0f,Space.Self);
        // WFR.transform.Rotate(torque,0f,0f,Space.Self);
        // WRL.transform.Rotate(torque,0f,0f,Space.Self);
        // WRR.transform.Rotate(torque,0f,0f,Space.Self);

        /*UpdateSingleWheel(FL,WFL);
        UpdateSingleWheel(FR,WFR);
        UpdateSingleWheel(RL,WRL);
        UpdateSingleWheel(RR,WRR);*/
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        // Vector3 position;
        // Quaternion rotation;
        //
        // wheelCollider.GetWorldPose(out position, out rotation);
        //
        // wheelTransform.rotation = rotation;
        // wheelTransform.position = position;
    }
}
