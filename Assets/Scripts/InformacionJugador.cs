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

    [Header("Gestión de la interfaz")] 
    private InterfazController _interfazController;
    public bool activacionProhibicion = false;
    
    private void Awake()
    {
        _interfazController = GameObject.Find("--INTERFAZ DEL USUARIO--").GetComponent<InterfazController>();
        
        /*
        etiquetaNombre = GameObject.Find("NombreJugador").GetComponent<TextMesh>();
        etiquetaNombre.text = nombreJugador;
        */
    }
    

    // Start is called before the first frame update
    void Start()
    {
        _posicionCarreraController = FindObjectOfType<PosicionCarreraController>();
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            _interfazController.ActualizaPosicion(posicionActual);
            _interfazController.ActualizaNumVueltas(vueltaActual, nVueltasCircuito);
            _interfazController.senalProhibicion(activacionProhibicion);
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

    /*
    private void OnTriggerExit(Collider collision)
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
    */
    
}
