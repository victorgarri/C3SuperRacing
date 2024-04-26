using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using ColorUtility = UnityEngine.ColorUtility;

public class InformacionJugador : NetworkBehaviour
{
    public int vueltas;
    
    [Header("Nombre del jugador")] 
    [SerializeField] public string nombreJugador = "Carlitos";
    public TextMesh etiquetaNombre;
    [SerializeField] public Color colorJugador;
    
    [Header("Gestión de las posiciones")]
    public PosicionCarreraController _posicionCarreraController;
    
    [SyncVar]
    public int posicionActual = 0;
    [SyncVar]
    public int vueltaActual = 0;
    public int nVueltasCircuito = 0;
    [SyncVar]
    public int nWaypoints = 0;
    [SyncVar]
    public int siguienteWaypoint = 0;
    [SyncVar]
    public float distanciaSiguienteWaypoint = 0;
    public float posicionAnterior;

    [Header("Gestión de la interfaz")] 
    private InterfazController _interfazController;

    public CarController _carController;

    [SyncVar] public Nullable<int> lastMinigameScore = null;
    
    
    public List<int> listaPuntuacionCarrera;
    public int puntuacionTotalCarrera = 0;
    public int indiceCarrera = 0;

    [SyncVar]
    public bool finCarrera=true;

    private SonidoFondo _sonidoFondo;
    
    private void Awake()
    {
        
        /*
        etiquetaNombre = GameObject.Find("NombreJugador").GetComponent<TextMesh>();
        etiquetaNombre.text = nombreJugador;
        */
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
    public void SetVueltaActual(int n)
    {
        vueltaActual = n;
    }
    

    // Start is called before the first frame update
    void Start()
    {
        listaPuntuacionCarrera = new List<int>();
        listaPuntuacionCarrera.Add(0);
        listaPuntuacionCarrera.Add(0);
        listaPuntuacionCarrera.Add(0);
        
        _interfazController = FindObjectOfType<GameManager>().interfazUsuario.GetComponent<InterfazController>();
        
        _posicionCarreraController = FindObjectOfType<PosicionCarreraController>();
        _carController = GetComponent<CarController>();
        _sonidoFondo = FindObjectOfType<SonidoFondo>().gameObject.GetComponent<SonidoFondo>();
    }

    void Update()
    {
        if (isLocalPlayer && _carController.enableControls)
        {
            float distanciaSiguienteWaypointAproximado =  Mathf.Round(distanciaSiguienteWaypoint * 100f) / 100f;
            float posicionAnteriorAproximado = Mathf.Round(posicionAnterior * 100f) / 100f;
            
            if (distanciaSiguienteWaypointAproximado < posicionAnteriorAproximado)
            {
                _interfazController.desactivarProhibicion();                
            }
            else if(distanciaSiguienteWaypointAproximado > posicionAnteriorAproximado)
            {
                if (!_interfazController.corBool)
                {
                    _interfazController.stopCor=StartCoroutine(_interfazController.activarProhibicion());
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
            SetVueltaActual(vueltaActual);
        }
    }

    public void ActualizarPuntuacionJugadorCarrera(int puntosConseguidos)
    {
        listaPuntuacionCarrera[indiceCarrera - 1] = puntosConseguidos;
        puntuacionTotalCarrera += listaPuntuacionCarrera[indiceCarrera - 1];
    }



    [Command]
    public void SetMinigameScore(Nullable<int> score)
    {
        this.lastMinigameScore = score;
    }

    [Command]
    public void CmdSetFinCarrera(bool finish)
    {
        this.finCarrera = finish;
    }

    
}
