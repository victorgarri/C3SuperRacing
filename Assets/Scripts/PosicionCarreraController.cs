using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PosicionCarreraController  : MonoBehaviour
{
    public List<Transform> listaWaypoints = new List<Transform>();
    private int vueltasTotales = 2;
    
    public int totalPlayers;
    InformacionJugador[] _informacionJugadores; //Esta variable se puede cambiar más adelante
    public TextMeshProUGUI tablaPosicion;

    private Dictionary<InformacionJugador, string> nombreJugador = new Dictionary<InformacionJugador, string>();
    private Dictionary<InformacionJugador, int> vueltaActualJugador;     //Diccionario para almacenar la vuelta actual de cada jugador
    private Dictionary<InformacionJugador, int> waypointTotalesJugador;  //Diccionario para almacenar los waypoints totales de cada jugador
    private Dictionary<InformacionJugador, float> distanciaWaypoint;     //Diccionario para almacenar la distancia del próximo waypoint
    
    private Dictionary<InformacionJugador, int> indiceWaypoint;          //Diccionario para almacenar el indice del proximo waypoint del jugador
    void Start()
    {
        vueltaActualJugador = new Dictionary<InformacionJugador, int>();
        waypointTotalesJugador = new Dictionary<InformacionJugador, int>();
        indiceWaypoint = new Dictionary<InformacionJugador, int>();
        
        distanciaWaypoint = new Dictionary<InformacionJugador, float>();
        
        StartCoroutine(EsperaColocacionPosiciones());
    }

    void Update(){
        GestionCambioPosicion();
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
        }
        
        totalPlayers=_informacionJugadores.Length;
	}

    void GestionCambioPosicion(){
        if(_informacionJugadores!=null){
            if(_informacionJugadores.Length>0){
                
                //Ordena mi lista
                InformacionJugador[] listaJugadoresOrdenados = _informacionJugadores.OrderByDescending(i => i.circuitosCompletados).
                                                                             ThenByDescending(i => i.vueltaActualJugador).
                                                                             ThenByDescending(i => i.puntoControlJugador).ToArray();
                //Lo visualizo en el panel
                int contador=1;
                tablaPosicion.text="";
                foreach(InformacionJugador orden in listaJugadoresOrdenados){
                    tablaPosicion.text+=contador + "º " +orden.nombre+"\n";
                    orden.posicionJugador=contador;
                    contador++;                    
                }
            }
        }
    }

    public void gestionCambioWaypoints(InformacionJugador jugador)
    {
        jugador.puntoControlJugador++;
        indiceWaypoint[jugador]++;
        
        if (indiceWaypoint[jugador] >= listaWaypoints.Count)
        {
            jugador.vueltaActualJugador++;
            if (jugador.vueltaActualJugador > vueltasTotales)
            {
                Debug.Log("Carrera hecha");
            }
            else
            {
                indiceWaypoint[jugador] = 0;
            }
        }
    }


}
    /*
    public List<Transform> waypoints; // Lista de los puntos de control en la pista
    public List<CarController> jugadores; // Lista de todos los jugadores en la carrera
    private Dictionary<CarController, int> posicionJugador; // Diccionario para almacenar las posiciones de los jugadores

    private Dictionary<CarController, int> vueltasJugador; // Diccionario para almacenar las vueltas actuales de cada jugador
    private Dictionary<CarController, int> waypointJugador; // Diccionario para almacenar el waypoint actual de cada jugador


    private int waypointIndex;
    private int numVueltasTotal = 2;
    // Start is called before the first frame update
    void Start()
    {


        posicionJugador = new Dictionary<CarController, int>();

        vueltasJugador = new Dictionary<CarController, int>();
        waypointJugador = new Dictionary<CarController, int>();
        InicioPosicionesJugadores();
    }

    public void InicioPosicionesJugadores()
    {
        CarController[] jugadoresEncontrados = FindObjectsByType<CarController>(0);
        Debug.Log(jugadoresEncontrados.Length);

        foreach (CarController jugadorAgregados in jugadoresEncontrados)
        {
            jugadores.Add(jugadorAgregados);
        }

        foreach (CarController jugador in jugadores)
        {
            posicionJugador.Add(jugador, 0); // Inicializa todas las posiciones de los jugadores como 0 al principio
            vueltasJugador.Add(jugador, 1); // Inicializa el número de vueltas completadas como 1
            waypointJugador.Add(jugador, 0); // Inicializa el número del waypoint
        }
    }

    // Update is called once per frame
    void Update()
    {
        ActualizoPosicionJugador();
    }

    public void ActualizoPosicionJugador()
    {
        foreach (CarController jugador in jugadores)
        {
            // Calcula la posición basada en las vueltas y los waypoints
            int position = vueltasJugador[jugador] * waypointJugador[jugador] + DistanciaWaypointCercano(jugador);

            posicionJugador[jugador] = position; // Actualiza la posición del jugador en el diccionario
        }

        // Ordena a los jugadores según su posición
        List<CarController> sortedPlayers = jugadores.OrderBy(jugador => posicionJugador[jugador]).ToList();
    }

    public int DistanciaWaypointCercano(CarController jugador)
    {

        waypointIndex = waypointJugador[jugador] + 1;

        float distance = Vector3.Distance(jugador.transform.position, waypoints[waypointIndex].position);

        return Mathf.RoundToInt(distance);
    }

    public void actualizoWaypoint(CarController jugador)
    {
        if (jugadores.Contains(jugador))
        {
            // Actualiza el waypoint del jugador en el diccionario
            waypointJugador[jugador]++;

            // Comprueba si el jugador ha pasado por el último waypoint
            if (waypointJugador[jugador] == waypoints.Count - 1)
            {
                // Incrementa el contador de vueltas para ese jugador
                vueltasJugador[jugador]++;

                if (vueltasJugador[jugador] > numVueltasTotal)
                {
                    Debug.Log("Hecho");
                }
                else
                {
                    //Reinicio el contador de waypoints
                    waypointJugador[jugador] = 0;
                }
            }
        }
    }
    */
