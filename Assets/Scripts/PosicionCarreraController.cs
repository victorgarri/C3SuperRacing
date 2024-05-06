using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Mirror;
using UnityEngine;
using UnityEngine.Serialization;

public class PosicionCarreraController : NetworkBehaviour
{
    [Header("Lista de Waypoints")]
    public List<Transform> listaWaypoints;
    
    [Header("Número de vueltas totales")]
    [SerializeField] public int vueltasTotales = 2;
    
    [Header("Recogemos el script de información del jugador")]
    public InformacionJugador[] _informacionJugadores;
    
    [Header("Colocación coches final de cada carrera")]
    [SerializeField] private List<Transform> spawnsFinales = new List<Transform>();
    private int sumaOrden = 0;
    public int puntuacionMaxima = 0;

    [FormerlySerializedAs("_tablaPosicionModoEspectador")]
    [Header("Script de mostrar tabla de posición modo espectador")] 
    [SerializeField] private InterfazUsuarioModoEspectador interfazUsuarioModoEspectador;
    
    [SerializeField]
    private GameManager _gameManager;

    
    // Start is called before the first frame update
    void Start()
    {
        listaWaypoints = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            listaWaypoints.Add(transform.GetChild(i));
        }
    }

    

    public float CalculoDistanciaSiguienteWaypoint(InformacionJugador jugador, int indiceSiguienteWaypoint)
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
        if (isServer)  
        {
            ActualizarPosiciones();   
        }
    }

    [Server]
    private void ActualizarPosiciones()
    {
        //Me voy actualizando en cada frame la distancia
        foreach (var jugador in _informacionJugadores)
        {
            jugador.distanciaSiguienteWaypoint = CalculoDistanciaSiguienteWaypoint(jugador, jugador.siguienteWaypoint);
        }

        var previousFirst = _informacionJugadores[0];
        //Me ordeno mi lista de jugadores
        _informacionJugadores = _informacionJugadores.OrderByDescending(jugador => jugador.vueltaActual).
                                                      ThenByDescending(jugador => jugador.nWaypoints).
                                                      ThenBy(jugador => jugador.distanciaSiguienteWaypoint).ToArray();
        
        PosicionarJugadores(_informacionJugadores);
        interfazUsuarioModoEspectador.actualizarTablaPosicion(_informacionJugadores);
    }

    private void PosicionarJugadores(InformacionJugador[] jugadores)
    {
        int orden = 1;
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
            if (jugador.nWaypoints > listaWaypoints.Count)
            {

                if (jugador.vueltaActual == vueltasTotales)
                {
                    //Cambiar cuando acabe la carrera
                    // 
                    jugador.finCarrera = true;
                    jugador.CmdSetFinCarrera(true);
                    _gameManager.ActualizarPuntuacionJugadorCarrera(jugador,puntuacionMaxima - 2 * (jugador.posicionActual-1));
                    
                    TargetFinishRace(jugador,jugador.posicionActual-1);

                }
                else
                {                    
                    jugador.vueltaActual++;
                    jugador.nWaypoints = 1;
                    jugador.siguienteWaypoint = 1;
                }
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

    
  
    public void TargetFinishRace(InformacionJugador target, int sumOrd)
    {
        target.transform.position = spawnsFinales[sumOrd].transform.position;
        target.transform.rotation = spawnsFinales[sumOrd].transform.rotation;
        target.gameObject.GetComponent<InformacionJugador>()._carController.DesactivateCar();
    }
    
}
