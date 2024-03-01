using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

public class InformacionJugador : MonoBehaviour
{
    public string nombre;

    public int posicionJugador;
    public int vueltaActualJugador;
    public int puntoControlJugador;

    private int anteriorWaypoint;
    
    public bool checkpointValido = true;
    
    private PosicionCarreraController _posicionCarreraController;
    // Start is called before the first frame update
    void Start()
    {
        _posicionCarreraController = FindObjectOfType<PosicionCarreraController>();
        anteriorWaypoint = _posicionCarreraController.listaWaypoints.Count - 1;
    }
    
    public void GestionActivacionYDesactivacionWaypoints(int siguienteWaypoint)
    {
        //Activo siguiente waypoint
        _posicionCarreraController.listaWaypoints[siguienteWaypoint].gameObject.SetActive(true);
        Debug.Log(siguienteWaypoint);
        
        //Guardo el anterior waypoint
        anteriorWaypoint = siguienteWaypoint - 1;
        if (anteriorWaypoint < 0)
        {
            anteriorWaypoint = _posicionCarreraController.listaWaypoints.Count - 1;
        }
        
        //Desactivo el postanterior waypoint
        int postAnteriorWaypoint = siguienteWaypoint - 2;
        if (postAnteriorWaypoint < 0)
        {
            postAnteriorWaypoint = _posicionCarreraController.listaWaypoints.Count - 2;
        }
        _posicionCarreraController.listaWaypoints[postAnteriorWaypoint].gameObject.SetActive(false);

    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Waypoint"))
        {
            //Si el nombre del waypoint coincide con el del anterior waypoint que pillo, se dará la vuelta
            //Así evitamos que los juagores hagan una vuelta atrás
            if(collision.gameObject.name == _posicionCarreraController.listaWaypoints[anteriorWaypoint].gameObject.name)
            {
                transform.rotation =
                    Quaternion.Euler(transform.position.x, transform.position.y + 180, transform.position.z);
            }
            else
            {
                _posicionCarreraController.gestionCambioWaypoints(this);
            }
        }
    }
}
