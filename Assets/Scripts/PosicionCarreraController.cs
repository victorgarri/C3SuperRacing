using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PosicionCarreraController  : MonoBehaviour
{
    //Parámetros del circuito, lista de Waypoits pata controlar posiciones de los jugadores y numero de vueltas totales que tendrá el circuito.
    public List<Transform> listaWaypoints = new List<Transform>();
    [SerializeField] public int vueltasTotales = 2;
    
    public int totalPlayers;
    InformacionJugador[] _informacionJugadores; //Esta variable se puede cambiar más adelante
    public TextMeshProUGUI tablaPosicion;

    private Dictionary<InformacionJugador, string> nombreJugador = new Dictionary<InformacionJugador, string>();
    private Dictionary<InformacionJugador, int> vueltaActualJugador;     //Diccionario para almacenar la vuelta actual de cada jugador
    private Dictionary<InformacionJugador, int> waypointTotalesJugador;  //Diccionario para almacenar los waypoints totales de cada jugado
    private Dictionary<InformacionJugador, float> distanciaWaypointCercano;     //Diccionario para almacenar la distancia del próximo waypoint
    private Dictionary<InformacionJugador, int> indiceWaypoint;          //Diccionario para almacenar el indice del proximo waypoint del jugador
    void Start()
    {
        
        vueltaActualJugador = new Dictionary<InformacionJugador, int>();
        waypointTotalesJugador = new Dictionary<InformacionJugador, int>();
        indiceWaypoint = new Dictionary<InformacionJugador, int>();
        distanciaWaypointCercano = new Dictionary<InformacionJugador, float>();
        
        StartCoroutine(EsperaColocacionPosiciones());
    }

    void Update(){
        ActualizacionPosicion();
    }

    IEnumerator EsperaColocacionPosiciones()
    {
        //yield on a new YieldInstruction that waits for 1 second.
        yield return new WaitForSeconds(1);
        ColocacionPosiciones();
    }
    void ColocacionPosiciones(){
        _informacionJugadores = FindObjectsOfType(typeof(InformacionJugador)) as InformacionJugador[];
        foreach (var jugador in _informacionJugadores)
        {
            nombreJugador.Add(jugador, jugador.nombre);
            vueltaActualJugador.Add(jugador, 1);
            waypointTotalesJugador.Add(jugador, 0);
            indiceWaypoint.Add(jugador, 0);
            distanciaWaypointCercano.Add(jugador, DistanciaWaypointCercano(jugador, indiceWaypoint[jugador]));
        }
        
        totalPlayers=_informacionJugadores.Length;
	}

    void ActualizacionPosicion(){
        if(_informacionJugadores!=null){
            foreach (var jugador in _informacionJugadores)
            {
                distanciaWaypointCercano[jugador] = DistanciaWaypointCercano(jugador, indiceWaypoint[jugador]);
            }
            
            // Ordena a los jugadores según su posición
            InformacionJugador[] jugadoresOrdenados = _informacionJugadores.OrderByDescending(jugador => vueltaActualJugador[jugador]). //Me ordena por número de vueltas
                                                                                ThenByDescending(jugador => waypointTotalesJugador[jugador]). //Me ordena por numero de waypoints totales
                                                                                ThenBy(jugador => distanciaWaypointCercano[jugador]).ToArray(); //Me ordena por distancia cercana al siguiente waypoiny
            //Lo visualizo en el panel
            int contador = 1;
            tablaPosicion.text="";
            foreach(InformacionJugador orden in jugadoresOrdenados){ 
                tablaPosicion.text+=contador + "º " +orden.nombre+"\n";
                orden.posicionJugador = contador;
                contador++;                    
                }
        }
    }
    
    public int DistanciaWaypointCercano(InformacionJugador jugador, int indice)
    {

        int waypointSiguiente = indice + 1;

        if (waypointSiguiente >= listaWaypoints.Count)
        {
            waypointSiguiente = 0;
        }
        
        float distancia = Vector3.Distance(jugador.transform.position, listaWaypoints[waypointSiguiente].position);

        return Mathf.RoundToInt(distancia);
    }

    public void gestionCambioWaypoints(InformacionJugador jugador)
    {
        indiceWaypoint[jugador]++;
        jugador.puntoControlJugador = indiceWaypoint[jugador];
        
        if (indiceWaypoint[jugador] >= listaWaypoints.Count)
        {
            vueltaActualJugador[jugador]++;
            jugador.vueltaActualJugador = vueltaActualJugador[jugador];
            
            if (vueltaActualJugador[jugador] > vueltasTotales)
            {
                Debug.Log("Carrera hecha");
            }
            else
            {
                indiceWaypoint[jugador] = 0;
            }
        }
        
        jugador.GestionActivacionYDesactivacionWaypoints(indiceWaypoint[jugador]);
    }
}
