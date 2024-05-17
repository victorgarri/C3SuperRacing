using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.Audio;
using Random = UnityEngine.Random;


public class CarController : NetworkBehaviour
{
    [Header("Inputs")]
    [SerializeField] private bool isBreaking;
    [SerializeField] private float giro;
    [SerializeField] private float pedal;
    [SerializeField] private float cameraTurn;
    

    
    private const float MAXBREAKFORCE = 3000F;
    private float cameraOffset=0;
    private float cameraSpeed=4.5f;
    private float cameraTimestamp=0;

    [Header("Configuración motor")]
    [SerializeField] private float motorForce;
    [SerializeField] private float breakForce;

    [Header("Wheel Colliders")]
    [SerializeField] private WheelCollider WCFL;
    [SerializeField] private WheelCollider WCFR;
    [SerializeField] private WheelCollider WCRL;
    [SerializeField] private WheelCollider WCRR;

    private float currentSteerAngle;
    [Header("Configuración del giro")]
    [SerializeField] private float maxSteerAngle=20;
    [SerializeField] private float radius = 1;
    [SerializeField] private float wheelBase;
    [SerializeField] private float trackWidth;
    [SerializeField] private bool antiAckerman = false;
    
    [Header("Ruedas visuales")]
    [SerializeField] private Transform FL;
    [SerializeField] private Transform FR;
    [SerializeField] private Transform RL;
    [SerializeField] private Transform RR;
    
    [SerializeField] private bool _updateWheels;

    [Header("Velocímetro")] 
    private InterfazController _interfazController;
    private float velocidad = 0f;
    private const float VELOCIDADMAXIMA = 80f;

    [Header("Sonido")]
    private SonidoFondo _sonidoFondo;
    private AudioSource _audioSource;
    [SerializeField] private GameObject motorCoche;
    [SerializeField] private AudioSource _sonidoMotor;
    [SerializeField] private AudioClip sonidoCocheArranque;
    [SerializeField] private AudioClip sonidoCocheArrancadoYa;
    [SerializeField] private AudioClip sonidoCocheChocandoConOtro;
    [SerializeField] private AudioClip claxonCoche;
    [SerializeField] private AudioClip claxonCoche2;
    
    private Rigidbody _rigidbody;
    private PlayerInput _playerInput;
    [SerializeField]private GameObject _camera;
    public bool enableControls;

    [SerializeField]
    private List<GameObject> carLights;

    private InformacionJugador _informacionJugador;

    private void Start()
    {
        
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.centerOfMass = new Vector3(0, -.23f, 0.1f);
        _playerInput = GetComponent<PlayerInput>();

        _interfazController = FindObjectOfType<GameManager>().interfazUsuario.GetComponent<InterfazController>();
            
        // _cameraPivot = transform.Find("CameraPivot").gameObject;
        
        _audioSource = this.GetComponent<AudioSource>();
        _sonidoFondo = FindObjectOfType<SonidoFondo>().gameObject.GetComponent<SonidoFondo>();
        
        wheelBase = Mathf.Abs(FL.transform.position.z - RL.transform.position.z);
        trackWidth = Mathf.Abs(FR.transform.position.x - FL.transform.position.x);

        _informacionJugador = GetComponent<InformacionJugador>();
        
        DesactivateCar();
    }
    
    public void ActivateCar(float seconds)
    {
        if (isLocalPlayer)
            _camera.SetActive(true);
        _interfazController.gameObject.SetActive(true);
        StartCoroutine(EnableControlsCoroutine(seconds));
        
    }
    
    private IEnumerator EnableControlsCoroutine(float seconds)
    {
        if (seconds == 3)
        {
            //Efecto de sonido de arrancar motor
            EjecutarEfectoSonido(sonidoCocheArranque, 0.5f);
        
            //Efecto de sonido de motor arrancado
            yield return new WaitForSeconds(1);
            EjecutarEfectoSonido(sonidoCocheArrancadoYa, 0.5f);
        
            yield return new WaitForSeconds(2);
            enableControls = true;
            _sonidoFondo.ReproducirMusicaVelocidadNormal(_informacionJugador.indiceCarrera);
            motorCoche.gameObject.SetActive(true);
            
        } else if (seconds == 8)
        {
            //Efecto de sonido de arrancar motor
            yield return new WaitForSeconds(5);
            EjecutarEfectoSonido(sonidoCocheArranque, 0.5f);
        
            //Efecto de sonido de motor arrancado
            yield return new WaitForSeconds(1);
            EjecutarEfectoSonido(sonidoCocheArrancadoYa, 0.5f);
        
            yield return new WaitForSeconds(2);
            enableControls = true;
            _sonidoFondo.ReproducirMusicaVelocidadNormal(_informacionJugador.indiceCarrera);
            motorCoche.gameObject.SetActive(true);
        }
    }

    private void EjecutarEfectoSonido(AudioClip clip, float volumen)
    {
        _audioSource.PlayOneShot(clip, volumen);
    }

    [TargetRpc]
    public void TargetDesactivateCar()
    {
       DesactivateCar();
    }

    public void DesactivateCar()
    {
        enableControls = false;

        CmdSetGiro(0);
        CmdSetPedal(0);
        CmdSetIsBreaking(true);
        
        _camera.SetActive(false);
        _interfazController.gameObject.SetActive(false);
        
        //Para parar la música de fondo
        _sonidoFondo.PararMusicaFondo();
        
        //Paramos el motor del coche
        _sonidoMotor.Stop();
        motorCoche.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (enableControls)
        {
            if (isLocalPlayer)
            {
                _interfazController.AgujaVelocimetro(velocidad, VELOCIDADMAXIMA);
            }

            // _cameraPivot.transform.position = this.transform.position;
            // _cameraPivot.transform.rotation = Quaternion.Euler(0,this.transform.eulerAngles.y + (cameraOffset),0);
            
            if(_playerInput != null && _playerInput.actions != null && _playerInput.actions["Claxon"] != null)
            {
                //Botón para tocar el claxon del coche.
                //En Mando -> Botón L
                //En PC -> Tecla C
                if (_playerInput.actions["Claxon"].WasPressedThisFrame())
                {
                    EjecutarSonidoClaxon();
                }
            }
        }
        
    }

    private void EjecutarSonidoClaxon()
    {
        if (!_audioSource.isPlaying)
        {
            int numeroRandom = Random.Range(0, 101);

            if (numeroRandom < 90)
            {
                EjecutarEfectoSonido(claxonCoche, 0.5f);
            }
            else
            {
                EjecutarEfectoSonido(claxonCoche2, 0.5f);
            }
            
        }
    }
    
    private void FixedUpdate()
    {
        if(enableControls)
            GetInput();
        
        // HandleCamera();
        HandleMotor();
        HandleSteering();
        GearSound();
        
        if(_updateWheels)UpdateWheels();
    }
    
    private void GearSound()
    {
        //Sonido del motor coche arrancado
        _sonidoMotor.pitch = velocidad / VELOCIDADMAXIMA + 1;
    }

    private void HandleCamera()
    {
        if (cameraTurn == 0)
        {
            if (cameraTimestamp == 0) cameraTimestamp = Time.time;
            else if(Time.time-cameraTimestamp>=2)cameraOffset *= .99f;
        }
        else
        {
            cameraTimestamp = 0;
        }
        cameraOffset = cameraOffset + cameraTurn * cameraSpeed;
        if (cameraOffset > 180) cameraOffset -= 360;
        else if (cameraOffset < -180) cameraOffset += 360;
    }

    private void GetInput()
    {
        // giro = Input.GetAxis("Horizontal");
        // giro = _playerInput.actions["Steer"].ReadValue<float>();
    
        // pedal = Input.GetAxis("Vertical");
        // pedal = _playerInput.actions["Throtle"].ReadValue<float>();

        // cameraTurn = _playerInput.actions["Camera"].ReadValue<float>();
        
        // isBreaking = _playerInput.actions["Brake"].IsPressed();

        CmdSetGiro(_playerInput.actions["Steer"].ReadValue<float>());
        CmdSetPedal(_playerInput.actions["Throtle"].ReadValue<float>());
        CmdSetIsBreaking(_playerInput.actions["Brake"].IsPressed());
    }
    
    [Command(requiresAuthority = false)]
    private void CmdSetGiro(float giroInput)
    {
        giro = giroInput;
    }
    [Command(requiresAuthority = false)]
    private void CmdSetPedal(float pedalInput)
    {
        pedal = pedalInput;
    }
    [Command(requiresAuthority = false)]
    private void CmdSetIsBreaking(bool breakingInput)
    {
        isBreaking = breakingInput;
    }
    
    
    
    private void HandleMotor()
    {
        velocidad = _rigidbody.velocity.magnitude * 3600 / 1000;
        if (Math.Abs(velocidad) < VELOCIDADMAXIMA) 
        {
            WCFL.motorTorque = motorForce/2*pedal;
            WCFR.motorTorque = motorForce/2*pedal;
        }
        else
        {
            WCFL.motorTorque = 0;
            WCFR.motorTorque = 0;
        }

        breakForce = isBreaking ? MAXBREAKFORCE : 0f;

        WCFL.brakeTorque = breakForce;
        WCFR.brakeTorque = breakForce;
        WCRL.brakeTorque = breakForce;
        WCRR.brakeTorque = breakForce;
        
    }

    private void HandleSteering()
    {
        // currentSteerAngle = maxSteerAngle * giro;
        // WCFL.steerAngle = currentSteerAngle;
        // WCFR.steerAngle = currentSteerAngle;
        if (giro > 0)
        {
            if (!antiAckerman)
            {
                WCFL.steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (radius + (trackWidth / 2))) * giro;
                WCFR.steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (radius - (trackWidth / 2))) * giro;   
            }
            else
            {
                WCFL.steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (radius - (trackWidth / 2))) * giro;
                WCFR.steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (radius + (trackWidth / 2))) * giro;
            }
        } else if (giro < 0)
        {
            if (!antiAckerman)
            {
                WCFL.steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (radius - (trackWidth / 2))) * giro;
                WCFR.steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (radius + (trackWidth / 2))) * giro;
            }
            else
            {
                WCFL.steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (radius + (trackWidth / 2))) * giro;
                WCFR.steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (radius - (trackWidth / 2))) * giro;
            }
        } else {
            WCFL.steerAngle = 0;
            WCFR.steerAngle = 0;
        }
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(WCFL,FL);
        UpdateSingleWheel(WCFR,FR);
        UpdateSingleWheel(WCRL,RL);
        UpdateSingleWheel(WCRR,RR);
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
                EjecutarEfectoSonido(sonidoCocheChocandoConOtro, 0.5f);
        }
    }
    
    [Command(requiresAuthority = false)]
    public void CmdMoveCar(int raceIndex,int spawnIndex)
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

    [Command(requiresAuthority = false)]
    public void CmdSetPositionRotation(Vector3 transformPosition, Quaternion transformRotation)
    {
        this.transform.position = transformPosition;
        this.transform.rotation = transformRotation;
        Physics.SyncTransforms();
    }
}
