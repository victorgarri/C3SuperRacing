using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using TMPro;
using UnityEngine;

public class InformacionJugador : NetworkBehaviour
{
    
    public int posicionJugador;
    public int vueltaActualJugador;
    public int puntoControlJugador;
    
    [Header("Gestión de waypoints")]
    private PosicionCarreraController _posicionCarreraController;
    private int anteriorWaypoint;
    
    [Header("Gestión texto posición")]
    private TextMeshProUGUI textoPosicion;
    
    // Start is called before the first frame update
    void Start()
    {
        _posicionCarreraController = FindObjectOfType<PosicionCarreraController>();
        textoPosicion = GameObject.Find("TextoPosicion").GetComponent<TextMeshProUGUI>();
        
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

    public void ActualizaPosicion(int posicion)
    {
        switch (posicion)
        {
            case 1:
                textoPosicion.color = HexToColor("#FFD700"); //Color dorado
                break;
            case 2:
                textoPosicion.color = HexToColor("#BEBEBE"); //Color plateado
                break;
            case 3:
                textoPosicion.color = HexToColor("#CD7F32"); //Color bronce
                break;
            case 4:
                textoPosicion.color = HexToColor("#FFFFFF"); //Color blanco
                break;
            case 5:
                textoPosicion.color = HexToColor("#ffdfd4"); //Color tono rojo suave
                break;
            case 6:
                textoPosicion.color = HexToColor("#ff7b5a"); //Color tono rojo medio
                break;
            case 7:
                textoPosicion.color = HexToColor("#ff5232"); //Color tono rojo duro
                break;
            case 8:
                textoPosicion.color = HexToColor("#ff0000"); //Color rojo puro
                break;
        }

        textoPosicion.text = posicion + ".º";
    }
    Color HexToColor(string hex)
    {
        Color color = new Color();
        ColorUtility.TryParseHtmlString(hex, out color);
        return color;
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
