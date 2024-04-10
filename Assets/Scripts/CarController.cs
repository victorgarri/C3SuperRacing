using System;
using System.Collections;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;


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
    private InterfazController _interfazController;
    private float velocidad = 0f;
    private const float VELOCIDADMAXIMA = 80f;

    [Header("Sonido")] 
    private float volumen = 5.0f;
    [SerializeField] private AudioClip sonidoCocheArranque;
    [SerializeField] private AudioClip sonidoCocheArrancadoYa;
    [SerializeField] private AudioClip sonidoCocheCorriendo;
    private bool acelerando = false;
    [SerializeField] private AudioClip sonidoCocheFrenando;
    [SerializeField] private AudioClip sonidoCocheChocandoConOtro;
    [SerializeField] private AudioClip claxonCoche;
    [SerializeField] private AudioClip musicaCircuito1;
    private bool musicaActivada = false;
    
    private Rigidbody _rigidbody;
    private PlayerInput _playerInput;
    private GameObject _cameraPivot;
    public bool enableControls;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.centerOfMass = new Vector3(0, -.23f, 0.1f);
        _playerInput = GetComponent<PlayerInput>();

        _interfazController = FindObjectOfType<GameManager>().interfazUsuario.GetComponent<InterfazController>();
            
        _cameraPivot = transform.Find("CameraPivot").gameObject;
        
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
        AudioListener.volume = volumen; //Le subo el volumen al audio
        AudioSource.PlayClipAtPoint(sonidoCocheArranque, this.transform.position);
        InvokeRepeating("SonidoCocheArrancadoBucle", 0f, sonidoCocheArrancadoYa.length);
        
        yield return new WaitForSeconds(seconds);
        CancelInvoke("SonidoCocheArrancadoBucle");
        enableControls = true;
    }

    private void SonidoCocheArrancadoBucle()
    {
        AudioSource.PlayClipAtPoint(sonidoCocheArrancadoYa, transform.position);
    }

    public void DesactivateCar()
    {
        enableControls = false;        
        _cameraPivot.SetActive(false);
        _interfazController.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (enableControls)
        {
            if (!musicaActivada)
            {
                InvokeRepeating("ReproduccionMusicaBucle", 0f, musicaCircuito1.length);
                musicaActivada = true;   
            }

            /*
            if (pedal > 0 && !acelerando)
            {
                acelerando = true;
                InvokeRepeating("SonidoCocheCorriendo", 0f, sonidoCocheCorriendo.length);
            }
            else
            {
                acelerando = false;
                CancelInvoke("SonidoCocheCorriendo");
            }
            */
            
            if (isLocalPlayer)
            {
                _interfazController.AgujaVelocimetro(velocidad, VELOCIDADMAXIMA);
            }

            _cameraPivot.transform.position = this.transform.position;
            _cameraPivot.transform.rotation = Quaternion.Euler(0,this.transform.eulerAngles.y + (cameraOffset),0);
        }
        else
        {
            CancelInvoke("ReproduccionMusicaBucle");
            musicaActivada = false;
        }
        
    }

    private void ReproduccionMusicaBucle()
    {
        AudioListener.volume = volumen / 2;
        AudioSource.PlayClipAtPoint(musicaCircuito1, transform.position);
    }
    
    private void SonidoCocheCorriendo()
    {
        AudioListener.volume = volumen / 5;
        AudioSource.PlayClipAtPoint(sonidoCocheCorriendo, transform.position);
    }

    private void FixedUpdate()
    {
        if (isLocalPlayer&&enableControls)
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            AudioListener.volume = volumen / 2; //Le subo el volumen al audio
            AudioSource.PlayClipAtPoint(sonidoCocheChocandoConOtro, this.transform.position);
        }
    }
    
    [TargetRpc]
    public void TargetMoveCar(int raceIndex,int spawnIndex)
    {
        var spawnTrasnform = FindObjectOfType<GameManager>().spawnPoints[raceIndex][spawnIndex].transform;
        this.transform.position = spawnTrasnform.position;
        this.transform.rotation = spawnTrasnform.rotation;
    }
}
