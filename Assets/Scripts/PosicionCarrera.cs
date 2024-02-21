using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PosicionCarrera  : MonoBehaviour
{
    public List<Transform> waypoints; // Lista de los puntos de control en la pista
    public List<CarController> jugadores; // Lista de todos los jugadores en la carrera
    private Dictionary<CarController, int> posicionJugador; // Diccionario para almacenar las posiciones de los jugadores
    
    private Dictionary<CarController, int> vueltasJugador; // Diccionario para almacenar las vueltas actuales de cada jugador
    private Dictionary<CarController, int> waypointJugador; // Diccionario para almacenar el waypoint actual de cada jugador
    /*
    public class valoresJugador
    {
        public int vueltasJugador;
        public int waypointJugador;
        public float distanciaWaypointCercano;
    }
    */
    
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
        
        CarController[] jugadoresEncontrados = GameObject.FindObjectsOfType<CarController>();

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
    
    
}
