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
    
    [Header("Gestión de las posiciones")]
    private PosicionCarreraController _posicionCarreraController;
    public int posicionActual = 0;
    public int vueltaActual = 0;
    public int nVueltasCircuito = 0;
    public int nWaypoints = 0;
    public int siguienteWaypoint = 0;
    public float distanciaSiguienteWaypoint = 0;
    public float posicionAnterior;

    [Header("Gestión de la interfaz")] 
    private InterfazController _interfazController;

    public CarController _carController;

    [SyncVar] public Nullable<int> lastMinigameScore = null;
    public List<int> listaPuntuacionCarrera;
    public int puntuacionTotalCarrera = 0;
    public int indiceCarrera = 0;
    
    private void Awake()
    {
        
        /*
        etiquetaNombre = GameObject.Find("NombreJugador").GetComponent<TextMesh>();
        etiquetaNombre.text = nombreJugador;
        */
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
    }

    void Update()
    {
        if (isLocalPlayer && _carController.enableControls)
        {
            float distanciaSiguienteWaypointAproximado =  Mathf.Round(distanciaSiguienteWaypoint * 100f) / 100f;
            float posicionAnteriorAproximado = Mathf.Round(posicionAnterior * 100f) / 100f;
            
            if (distanciaSiguienteWaypointAproximado <= posicionAnteriorAproximado)
            {
                _interfazController.desactivarProhibicion();
            }
            else
            {
                StartCoroutine(_interfazController.activarProhibicion());
            }
            posicionAnterior = distanciaSiguienteWaypoint;
            
            _interfazController.ActualizaPosicion(posicionActual);
            _interfazController.ActualizaNumVueltas(vueltaActual, nVueltasCircuito);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Waypoint"))
        {
            if(collision.gameObject.name == _posicionCarreraController.listaWaypoints[siguienteWaypoint].gameObject.name)
            {
                _posicionCarreraController.ActualizacionWaypoints(this, siguienteWaypoint);
            }
            else
            {
                _posicionCarreraController.ActualizacionWaypoints(this, siguienteWaypoint - 1);   
            }
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
}
