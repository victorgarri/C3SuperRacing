using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.Audio;


public class CarController : NetworkBehaviour
{
    [SerializeField]
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
    private InterfazController _interfazController;
    private float velocidad = 0f;
    private const float VELOCIDADMAXIMA = 80f;

    [Header("Sonido")]
    private float volumen = 5.0f;
    [SerializeField] private AudioClip sonidoCocheArranque;
    [SerializeField] private AudioClip sonidoCocheArrancadoYa;
    [SerializeField] private AudioClip sonidoCocheChocandoConOtro;
    [SerializeField] private AudioClip SonidoCocheCorriendo;
    [SerializeField] private AudioClip claxonCoche;
    [SerializeField] private AudioClip musicaCircuito1;
    [SerializeField] private AudioClip musicaCircuito2;
    [SerializeField] private AudioClip musicaCircuito3;
    [SerializeField] private AudioSource sonidoMeta;
    private bool musicaActivada = false;
    private SonidoFondo _sonidoFondo;

    private AudioSource _audioSource;
    
    private Rigidbody _rigidbody;
    private PlayerInput _playerInput;
    private GameObject _cameraPivot;
    public bool enableControls;

    [SerializeField]
    private List<GameObject> carLights;

    private void Start()
    {
        
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.centerOfMass = new Vector3(0, -.23f, 0.1f);
        _playerInput = GetComponent<PlayerInput>();

        _interfazController = FindObjectOfType<GameManager>().interfazUsuario.GetComponent<InterfazController>();
            
        _cameraPivot = transform.Find("CameraPivot").gameObject;

        _audioSource = this.GetComponent<AudioSource>();
        _sonidoFondo = FindObjectOfType<SonidoFondo>().gameObject.GetComponent<SonidoFondo>();
        
        wheelBase = Mathf.Abs(FL.transform.position.z - RL.transform.position.z);
        trackWidth = Mathf.Abs(FR.transform.position.x - FL.transform.position.x);
        
        DesactivateCar();
    }
    
    public void ActivateCar(int seconds)
    {
        if (isLocalPlayer)
            _cameraPivot.SetActive(true);
        _interfazController.gameObject.SetActive(true);
        StartCoroutine(EnableControlsCoroutine(seconds));
        
    }
    
    private IEnumerator EnableControlsCoroutine(int seconds)
    {
        //Efecto de sonido de arrancar motor
        EjecutarEfectoSonido(sonidoCocheArranque, 1);

        //Efecto de sonido de motor arrancado
        yield return new WaitForSeconds(seconds - 2);
        EjecutarEfectoSonido(sonidoCocheArrancadoYa, 2);
        
        yield return new WaitForSeconds(seconds - 1);
        _sonidoFondo.ReproducirMusicaVelocidadNormal();
        enableControls = true;
    }

    private void EjecutarEfectoSonido(AudioClip clip, float volumen)
    {
        _audioSource.PlayOneShot(clip, volumen);
    }

    private void GearSound()
    {
        _audioSource.PlayOneShot(SonidoCocheCorriendo,  velocidad/VELOCIDADMAXIMA*0.5f);
    }

    public void DesactivateCar()
    {
        enableControls = false;

        giro = 0;
        pedal = 0;
        cameraInput = 0;
        isBreaking = true;
        
        _cameraPivot.SetActive(false);
        _interfazController.gameObject.SetActive(false);
        
        //Para parar la música de fondo
        _sonidoFondo.PararMusicaFondo();
    }

    private void Update()
    {
        if (enableControls)
        {
            if (isLocalPlayer)
            {
                _interfazController.AgujaVelocimetro(velocidad, VELOCIDADMAXIMA);
            }

            _cameraPivot.transform.position = this.transform.position;
            _cameraPivot.transform.rotation = Quaternion.Euler(0,this.transform.eulerAngles.y + (cameraOffset),0);
        }
        
    }
    
    private void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            if(enableControls)
                GetInput();
            
            HandleCamera();
            HandleMotor();
            HandleSteering();
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
        
        //GearSound();

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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if(isLocalPlayer) 
                EjecutarEfectoSonido(sonidoCocheChocandoConOtro, 1);
        }
    }
    
    [TargetRpc]
    public void TargetMoveCar(int raceIndex,int spawnIndex)
    {
        var spawnTrasnform = FindObjectOfType<GameManager>().spawnPoints[raceIndex][spawnIndex].transform;
        this.gameObject.transform.position = spawnTrasnform.position;
        this.gameObject.transform.rotation = spawnTrasnform.rotation;
        Physics.SyncTransforms();
    }

    [ClientRpc]
    public void SetCarLights(bool carLightBool)
    {
        foreach (var carLight in carLights)
        {
            carLight.SetActive(carLightBool);
        }
    }
}
