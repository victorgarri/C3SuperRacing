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

    private CarController _carController;

    [SyncVar] public Nullable<int> lastMinigameScore =null;
    
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
        _interfazController = FindObjectOfType<GameManager>().interfazUsuario.GetComponent<InterfazController>();
        
        _posicionCarreraController = FindObjectOfType<PosicionCarreraController>();
        _carController = GetComponent<CarController>();
    }

    void Update()
    {
        if (isLocalPlayer && _carController.enableControls)
        {
            if (distanciaSiguienteWaypoint < posicionAnterior)
            {
                _interfazController.desactivarProhibicion();
            }
            else
            {
                _interfazController.activarProhibicion();
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



    [Command]
    public void SetMinigameScore(Nullable<int> score)
    {
        this.lastMinigameScore = score;
    }
}
