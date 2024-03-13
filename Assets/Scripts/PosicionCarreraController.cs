using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PosicionCarreraController : MonoBehaviour
{
    [Header("Lista de Waypoints")]
    public List<Transform> listaWaypoints = new List<Transform>();
    
    [Header("Número de vueltas totales")]
    [SerializeField] public int vueltasTotales = 2;
    
    [Header("Recogemos el script de información del jugador para llevar a cabo unas gestiones. Actualizar posiciones y actualizar número de vueltas que lleva")]
    InformacionJugador[] _informacionJugadores; //Esta variable se puede cambiar más adelante
    private InformacionJugador localPlayer;
    
    [Header("Diccionarios para almacenar los valores que queremos ordenar para llevar a cabo el cambio de posiciones")]
    private Dictionary<InformacionJugador, int> vueltaActualJugador;            //Diccionario para almacenar la vuelta actual de cada jugador
    private Dictionary<InformacionJugador, float> distanciaWaypointCercano;     //Diccionario para almacenar la distancia del próximo waypoint
    private Dictionary<InformacionJugador, int> indiceSiguienteWaypoint;        //Diccionario para almacenar el indice del proximo waypoint del jugador

    private int sumaOrden = 1;
    void Start()
    {
        vueltaActualJugador = new Dictionary<InformacionJugador, int>();
        indiceSiguienteWaypoint = new Dictionary<InformacionJugador, int>();
        distanciaWaypointCercano = new Dictionary<InformacionJugador, float>();
        
        StartCoroutine(EsperaColocacionPosiciones());
    }
    
    private IEnumerator EsperaColocacionPosiciones()
    {
        yield return new WaitForSeconds(1);
        PreparacionPosicionesJugadores();
    }
    private void PreparacionPosicionesJugadores(){
        
        _informacionJugadores = FindObjectsOfType(typeof(InformacionJugador)) as InformacionJugador[];
        foreach (var jugador in _informacionJugadores)
        {
            vueltaActualJugador.Add(jugador, 1);
            indiceSiguienteWaypoint.Add(jugador, 0);
            distanciaWaypointCercano.Add(jugador, indiceSiguienteWaypoint[jugador]);

            if (jugador.isLocalPlayer)
            {
                localPlayer = jugador;
                jugador.ActualizaNumVueltas(vueltaActualJugador[jugador], vueltasTotales);
                jugador.vueltaActualJugador = vueltaActualJugador[jugador];
                jugador.siguienteWaypoint = indiceSiguienteWaypoint[jugador];
                jugador.anteriorWaypoint = listaWaypoints.Count - 2;
            }
        }
    }

    
    void Update(){
        ActualizarPosiciones();
    }
    
    private void ActualizarPosiciones(){
        if(_informacionJugadores != null){
            foreach (var jugador in _informacionJugadores)
            {
                distanciaWaypointCercano[jugador] = CalculoDistanciaWaypointCercano(jugador, indiceSiguienteWaypoint[jugador]);
            }
            
            
            // Ordena a los jugadores según su posición
            _informacionJugadores = _informacionJugadores.OrderBy(jugador => vueltaActualJugador[jugador]).
                                                          ThenByDescending(jugador => indiceSiguienteWaypoint[jugador]).
                                                          ThenBy(jugador => distanciaWaypointCercano[jugador]).ToArray(); 

            OrdenarJugadores(_informacionJugadores, sumaOrden);
        }
    }
    
    private float CalculoDistanciaWaypointCercano(InformacionJugador jugador, int indice)
    {
        float distancia = Vector3.Distance(jugador.transform.position, listaWaypoints[indice].position);
        jugador.distancia = distancia;
        
        return distancia;
    }

    private void OrdenarJugadores(InformacionJugador[] jugadores, int orden)
    {
        int numero = orden;
        foreach(InformacionJugador jugador in jugadores){
                
            //Si el jugador de la lista es el local, que me actualice la posicion solamente a ese jugador local
            if (jugador == localPlayer)
            {
                jugador.ActualizaPosicion(numero);
                jugador.posicion = numero;
            }  
            //Variable para probar
            numero++;
        }
    }
    

    public void GestionCambioWaypoints(InformacionJugador jugador)
    {
        indiceSiguienteWaypoint[jugador]++;
        
        if (indiceSiguienteWaypoint[jugador] >= listaWaypoints.Count)
        {
            vueltaActualJugador[jugador]++;
            //jugador.vueltaActualJugador = vueltaActualJugador[jugador];
            
            if (vueltaActualJugador[jugador] > vueltasTotales)
            {
                sumaOrden++;
                jugador.gameObject.SetActive(false);
                Debug.Log("Carrera hecha");
            }
            else
            {
                if (jugador == localPlayer)
                {
                    jugador.ActualizaNumVueltas(vueltaActualJugador[jugador], vueltasTotales);
                }
                
                indiceSiguienteWaypoint[jugador] = 0;
            }
        }

        if (jugador == localPlayer)
        {
            jugador.GestionControlWaypoints(indiceSiguienteWaypoint[jugador]);   
        }
        
    }
}
