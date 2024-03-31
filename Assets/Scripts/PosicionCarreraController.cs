using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PosicionCarreraController : MonoBehaviour
{
    [Header("Lista de Waypoints")]
    public List<Transform> listaWaypoints = new List<Transform>();
    
    [Header("Número de vueltas totales")]
    [SerializeField] public int vueltasTotales = 2;
    
    [Header("Recogemos el script de información del jugador")]
    InformacionJugador[] _informacionJugadores;
    private InformacionJugador localPlayer;
    
    private int sumaOrden = 1;
    
    // Start is called before the first frame update
    void Start()
    {
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
        float distancia = Vector3.Distance(jugador.transform.position, listaWaypoints[indiceSiguienteWaypoint].position);

        return distancia;
    }
    
    void Update()
    {
        // ActualizarPosiciones();
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
            jugador.activacionProhibicion = false;
            
            jugador.nWaypoints++;
            jugador.siguienteWaypoint++;

            //Si pilla todos los waypoints, que me sume una vuelta
            if (jugador.nWaypoints >= listaWaypoints.Count)
            {
                jugador.vueltaActual++;

                if (jugador.vueltaActual > vueltasTotales)
                {
                    //Cambiar cuando acabe la carrera
                    sumaOrden++;
                    jugador.gameObject.SetActive(false);
                    Debug.Log("Carrera hecha");
                }

                jugador.nWaypoints = 0;
                jugador.siguienteWaypoint = 0;
            }
        }
        else
        {
            jugador.activacionProhibicion = true;
            
            jugador.nWaypoints--;
            jugador.siguienteWaypoint--;

            if (jugador.nWaypoints < 0)
            {
                if (jugador.nWaypoints == -1)
                {
                    jugador.vueltaActual--;
                }
                jugador.siguienteWaypoint = listaWaypoints.Count + jugador.nWaypoints;
            }
        }
    }
}
