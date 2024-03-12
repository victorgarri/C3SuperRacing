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
    private Dictionary<InformacionJugador, int> waypointTotalesJugador;         //Diccionario para almacenar los waypoints totales de cada jugado
    private Dictionary<InformacionJugador, float> distanciaWaypointCercano;     //Diccionario para almacenar la distancia del próximo waypoint
    private Dictionary<InformacionJugador, int> indiceSiguienteWaypoint;        //Diccionario para almacenar el indice del proximo waypoint del jugador
    void Start()
    {
        vueltaActualJugador = new Dictionary<InformacionJugador, int>();
        waypointTotalesJugador = new Dictionary<InformacionJugador, int>();
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
            waypointTotalesJugador.Add(jugador, 0);
            indiceSiguienteWaypoint.Add(jugador, 0);

            if (jugador.isLocalPlayer)
            {
                localPlayer = jugador;
                distanciaWaypointCercano.Add(jugador, jugador.DistanciaCercanaSiguienteWaypoint());
                jugador.ActualizaNumVueltas(vueltaActualJugador[jugador], vueltasTotales);
            }
        }
    }

    void Update(){
        ActualizarPosiciones();
    }

    private void ActualizarPosiciones(){
        if(_informacionJugadores!=null){
            foreach (var jugador in _informacionJugadores)
            {
                if (jugador == localPlayer)
                {
                    distanciaWaypointCercano[jugador] = jugador.DistanciaCercanaSiguienteWaypoint();
                }
            }
            
            // Ordena a los jugadores según su posición
            InformacionJugador[] jugadoresOrdenados = _informacionJugadores.OrderByDescending(jugador => vueltaActualJugador[jugador]).         //Me ordena por número de vueltas
                                                                                ThenByDescending(jugador => waypointTotalesJugador[jugador]).   //Me ordena por número de waypoints totales
                                                                                ThenBy(jugador => distanciaWaypointCercano[jugador]).ToArray(); //Me ordena por distancia cercana al siguiente waypoiny
            
            int contador = 1;
            foreach(InformacionJugador orden in jugadoresOrdenados){
                
                //Si el jugador de la lista es el local, que me actualice la posicion solamente a ese jugador local
                if (orden == localPlayer)
                {
                    orden.ActualizaPosicion(contador);                    
                }
                //orden.posicionJugador = contador; //Variable para probar
                contador++;                    
                }
            
        }
    }
    /*
    private int CalculoDistanciaWaypointCercano(InformacionJugador jugador, int indice)
    {
        int waypointSiguiente = indice + 1;

        if (waypointSiguiente >= listaWaypoints.Count)
        {
            waypointSiguiente = 0;
        }
        
        float distancia = Vector3.Distance(jugador.transform.position, listaWaypoints[waypointSiguiente].position);
        return Mathf.RoundToInt(distancia);
    }
    */

    public void GestionCambioWaypoints(InformacionJugador jugador)
    {
        waypointTotalesJugador[jugador]++;
        
        indiceSiguienteWaypoint[jugador]++;
        jugador.puntoControlJugador = indiceSiguienteWaypoint[jugador];
        
        if (indiceSiguienteWaypoint[jugador] >= listaWaypoints.Count)
        {
            vueltaActualJugador[jugador]++;
            //jugador.vueltaActualJugador = vueltaActualJugador[jugador];
            
            if (vueltaActualJugador[jugador] > vueltasTotales)
            {
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
            jugador.GestionActivacionYDesactivacionWaypoints(indiceSiguienteWaypoint[jugador]);   
        }
        
    }
}
