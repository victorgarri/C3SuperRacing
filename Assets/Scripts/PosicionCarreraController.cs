using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PosicionCarreraController : MonoBehaviour
{
    [Header("Lista de Waypoints")]
    public List<Transform> listaWaypoints;
    
    [Header("Número de vueltas totales")]
    [SerializeField] public int vueltasTotales = 2;
    
    [Header("Recogemos el script de información del jugador")]
    InformacionJugador[] _informacionJugadores;
    private InformacionJugador localPlayer;
    
    [Header("Colocación coches final de cada carrera")]
    [SerializeField] private List<Transform> spawnsFinales = new List<Transform>();
    private int sumaOrden = 1;
    
    // Start is called before the first frame update
    void Start()
    {
        listaWaypoints = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            listaWaypoints.Add(transform.GetChild(i));
        }
        
        //Reseteo los valores antes de empezar la carrera
        StartCoroutine(ReseteoVariablesJugadores());

    }

    private IEnumerator ReseteoVariablesJugadores()
    {
        yield return new WaitForSeconds(1);
        
        _informacionJugadores = FindObjectsOfType(typeof(InformacionJugador)) as InformacionJugador[];
        foreach (var jugador in _informacionJugadores)
        {
            jugador.vueltaActual = 1;
            jugador.nVueltasCircuito = vueltasTotales;
            jugador.nWaypoints = 0;
            jugador.siguienteWaypoint = 0;
            jugador.distanciaSiguienteWaypoint = CalculoDistanciaSiguienteWaypoint(jugador, jugador.siguienteWaypoint);
        }
    }

    private float CalculoDistanciaSiguienteWaypoint(InformacionJugador jugador, int indiceSiguienteWaypoint)
    {
        Vector3 coordenadasJugador = jugador.transform.position;
        BoxCollider waypointCollider = listaWaypoints[indiceSiguienteWaypoint].gameObject.GetComponent<BoxCollider>();

        //Para calcular la coordenada al punto más cercano del waypoint
        Vector3 waypointClosestPoint = waypointCollider.ClosestPoint(coordenadasJugador);
        
        //Para calcular la distancia al punto más cercano del waypoint
        float distancia = Vector3.Distance(coordenadasJugador, waypointClosestPoint);

        return distancia;
    }
    
    void Update()
    {
        if (_informacionJugadores != null)
        {
            ActualizarPosiciones();   
        }
    }

    private void ActualizarPosiciones()
    {
        //Me voy actualizando en cada frame la distancia
        foreach (var jugador in _informacionJugadores)
        {
            jugador.distanciaSiguienteWaypoint = CalculoDistanciaSiguienteWaypoint(jugador, jugador.siguienteWaypoint);
        }
        
        //Me ordeno mi lista de jugadores
        _informacionJugadores = _informacionJugadores.OrderByDescending(jugador => jugador.vueltaActual).
                                                      ThenByDescending(jugador => jugador.nWaypoints).
                                                      ThenBy(jugador => jugador.distanciaSiguienteWaypoint).ToArray();

        PosicionarJugadores(_informacionJugadores, sumaOrden);
    }

    private void PosicionarJugadores(InformacionJugador[] jugadores, int sumaOrden)
    {
        int orden = sumaOrden;
        foreach(InformacionJugador jugador in jugadores)
        {
            jugador.posicionActual = orden;
            orden++;
        }
    }

    public void ActualizacionWaypoints(InformacionJugador jugador, int indiceWaypoint)
    {
        //Si coincide el waypoint pillado, que me sume
        if (indiceWaypoint == jugador.siguienteWaypoint)
        {
            jugador.nWaypoints++;
            jugador.siguienteWaypoint= (jugador.siguienteWaypoint + 1)%listaWaypoints.Count;

            //Si pilla todos los waypoints, que me sume una vuelta
            if (jugador.nWaypoints >= listaWaypoints.Count)
            {
                jugador.vueltaActual++;

                if (jugador.vueltaActual > vueltasTotales)
                {
                    //Cambiar cuando acabe la carrera
                    jugador.transform.position = spawnsFinales[sumaOrden - 1].transform.position;
                    jugador.transform.rotation = spawnsFinales[sumaOrden - 1].transform.rotation;

                    sumaOrden++;
                }

                jugador.nWaypoints = 0;
                jugador.siguienteWaypoint = 0;
            }
        }
        else
        {
            jugador.nWaypoints--;
            jugador.siguienteWaypoint--;

            if (Math.Abs(jugador.nWaypoints) >= listaWaypoints.Count)
                jugador.nWaypoints = 0;
            
            if (jugador.nWaypoints < 0)
            {
                jugador.siguienteWaypoint = listaWaypoints.Count + jugador.nWaypoints;
            }
        }
    }
}
