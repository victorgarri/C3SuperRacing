using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using TMPro;
using UnityEditor.TextCore.Text;
using UnityEngine;
using UnityEngine.Serialization;
using ColorUtility = UnityEngine.ColorUtility;
using Image = UnityEngine.UI.Image;
using UnityEngine.InputSystem;

public class InformacionJugador : NetworkBehaviour
{
    public int vueltas;
    
    [Header("Nombre del jugador")] 
    [SerializeField] [SyncVar] public string nombreJugador = "Carlitos";
    public TextMesh etiquetaNombre;
    [SerializeField] public Color colorJugador;
    
    [Header("Gestión de las posiciones")]
    public PosicionCarreraController _posicionCarreraController;
    
    [SyncVar]
    public int posicionActual = 0;
    [SyncVar]
    public int vueltaActual = 0;
    public int nVueltasCircuito = 0;
    [SyncVar] public int nWaypoints = 0;
    [SyncVar] public int siguienteWaypoint = 0;
    [SyncVar] public float distanciaSiguienteWaypoint = 0;
    public float posicionAnterior;


    [Header("Gestión de la interfaz")] public InterfazController _interfazController;

    public CarController _carController;

    [SyncVar] public Nullable<int> lastMinigameScore = null;
    
    public int indiceCarrera = 0;

    [SyncVar] public bool finCarrera = true;
    [SyncVar] public bool finMinijuego = false;

    private SonidoFondo _sonidoFondo;

    [SerializeField] private uint playerNetworkId;
    [SyncVar]public int playerIndex;
    public  Material[] colorMaterialByIndex;
    public Material playerColorMaterial;

    private void Awake()
    {

        /*
        etiquetaNombre = GameObject.Find("NombreJugador").GetComponent<TextMesh>();
        etiquetaNombre.text = nombreJugador;
        */
    }

    
    [Header("PowerUp")]
    public bool isPowerUpCollected = false;
    public GameObject proyectilPrefab;
    public Transform spawnPoint;
    public float velocidadProyectil = 20f;
    public Image powerUpImage;
    public Sprite powerUpSprite;
    public Sprite nonePowerUpSprite;
    private PlayerInput _playerInput;
    


    // Start is called before the first frame update
    void Start()
    {
        if (isLocalPlayer)
        {
            LocalPlayerPointer.Instance.gamePlayerGameObject = gameObject;
            SetNombreJugador(LocalPlayerPointer.Instance.roomPlayer.playerName);
        }

        SetMaterialJugador();
        
        _interfazController = FindObjectOfType<GameManager>().interfazUsuario.GetComponent<InterfazController>();

        _posicionCarreraController = FindObjectOfType<PosicionCarreraController>();
        _carController = GetComponent<CarController>();
        _sonidoFondo = FindObjectOfType<SonidoFondo>().gameObject.GetComponent<SonidoFondo>();
        
        _playerInput = GetComponent<PlayerInput>();

        _interfazController.imagenPowerUp.SetActive(true);
        powerUpImage = _interfazController.imagenPowerUp.GetComponent<Image>();
        powerUpImage.sprite = nonePowerUpSprite;
        this.playerNetworkId = netId;
    }

    private void SetMaterialJugador()
    {
        playerColorMaterial = colorMaterialByIndex[playerIndex];
        var renderers = GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            var materials = renderer.materials;
            
            for (int i = 0; i < materials.Length; i++)
                if (materials[i].name == "Color Base (Instance)")
                    materials[i] = playerColorMaterial;
            
            renderer.SetMaterials(new List<Material>(materials));
        }
        GetComponentInChildren<SpriteRenderer>().material = playerColorMaterial;
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space) && isPowerUpCollected == true)
        if(_playerInput.actions["Throw"].IsPressed() && isPowerUpCollected == true)
        {
            UsePowerUp();
            LanzarProyectil();
        }
        
        if (isLocalPlayer && _carController.enableControls)
        {
            float distanciaSiguienteWaypointAproximado = Mathf.Round(distanciaSiguienteWaypoint * 100f) / 100f;
            float posicionAnteriorAproximado = Mathf.Round(posicionAnterior * 100f) / 100f;

            if (distanciaSiguienteWaypointAproximado < posicionAnteriorAproximado)
            {
                _interfazController.desactivarProhibicion();
            }
            else if (distanciaSiguienteWaypointAproximado > posicionAnteriorAproximado)
            {
                if (!_interfazController.corBool)
                {
                    _interfazController.stopCor = StartCoroutine(_interfazController.activarProhibicion());
                }
            }

            posicionAnterior = distanciaSiguienteWaypoint;

            if (vueltaActual == nVueltasCircuito)
            {
                _sonidoFondo.ReproducirMusicaVelocidadRapida();
            }

            _interfazController.ActualizaPosicion(posicionActual);
            _interfazController.ActualizaNumVueltas(vueltaActual, nVueltasCircuito);
        }
    }
    
    #region POWERUPS

    public void CollectPowerUp()
    {
        isPowerUpCollected = true;
        if (_interfazController.imagenPowerUp != null)
        {
            powerUpImage.sprite = powerUpSprite;
        }
    }

    private void UsePowerUp()
    {
        isPowerUpCollected = false;
        if (_interfazController.imagenPowerUp != null)
        {
            powerUpImage.sprite = nonePowerUpSprite;
        }
    }
    
    private void LanzarProyectil()
    {
        if (proyectilPrefab != null && spawnPoint != null)
        {
            GameObject proyectil = Instantiate(proyectilPrefab, spawnPoint.position, spawnPoint.rotation);
            Rigidbody proyectilRb = proyectil.GetComponent<Rigidbody>();
        
            Vector3 launchDirection = spawnPoint.forward;
    
            proyectilRb.velocity = launchDirection * velocidadProyectil;
        }
    }

    #endregion

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Waypoint") && isLocalPlayer)
        {
            // Debug.Log("SiguienteWaypoint: "+siguienteWaypoint);
            if(collision.gameObject.name == _posicionCarreraController.listaWaypoints[siguienteWaypoint].gameObject.name)
            {
                _posicionCarreraController.ActualizacionWaypoints(this, siguienteWaypoint);
            }
            else
            {
                _posicionCarreraController.ActualizacionWaypoints(this, siguienteWaypoint - 1);   
            }
            
            collision.gameObject.SetActive(false);
            
            int prevWaypoint = siguienteWaypoint-2;
            if (prevWaypoint < 0)
                prevWaypoint = _posicionCarreraController.listaWaypoints.Count+prevWaypoint;
            
            _posicionCarreraController.listaWaypoints[prevWaypoint].gameObject.SetActive(true);
            
            _posicionCarreraController.listaWaypoints[siguienteWaypoint].gameObject.SetActive(true);
            
            SetNWaypoints(nWaypoints);
            SetSiguienteWaypoint(siguienteWaypoint);
            
            //InformacionJugador SetVuelta
            Debug.Log("InformacionJugador SetVuelta");
            SetVueltaActual(vueltaActual);
        }
        
        if (collision.CompareTag("PowerUp"))
        {
            CollectPowerUp();
            Destroy(collision.gameObject);
        }
    }


    [Command]
    public void SetMinigameScore(Nullable<int> score)
    {
        this.lastMinigameScore = score;
    }
    
    [Command]
    public void SetVueltaActual(int n)
    {
        vueltaActual = n;
    }
    [Command]
    public void SetNWaypoints(int n)
    {
        nWaypoints = n;
    }
    [Command]
    public void SetSiguienteWaypoint(int i)
    {
        siguienteWaypoint = i;
    }
    [Command]
    public void CmdSetFinCarrera(bool finish)
    {
        this.finCarrera = finish;
    }
    [Command]
    public void CmdSetFinMinijuego(bool finish)
    {
        this.finMinijuego = finish;
    }
    [Command]
    public void SetNombreJugador(string playerName)
    {
        this.nombreJugador = playerName;
    }
}
