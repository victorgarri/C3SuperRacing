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
    
    [Header("Cuenta atrás")] 
    [SerializeField] [SyncVar] public bool cuentaAtrasActivado = false;
    [SerializeField] [SyncVar] public int segundosRestantes = 60;
    
    [Header("Recogemos el script de información del jugador")]
    public InformacionJugador[] _informacionJugadores;
    
    [Header("Colocación coches final de cada carrera")]
    [SerializeField] private List<Transform> spawnsFinales = new List<Transform>();
    private int sumaOrden = 0;
    public int puntuacionMaxima = 0;
    public int contadorTiempo = 0;
    private bool carreraFinalizada = false;
    
    [Header("Script del GameManager")]
    [SerializeField] private GameManager _gameManager;

    [FormerlySerializedAs("_tablaPosicionModoEspectador")]
    [Header("Script de mostrar tabla de posición modo espectador")] 
    [SerializeField] private InterfazUsuarioModoEspectador interfazUsuarioModoEspectador;
    
    
    // Start is called before the first frame update
    void Start()
    {
        listaWaypoints = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            listaWaypoints.Add(transform.GetChild(i));
        }

        StartCoroutine(Contador());

    }

    IEnumerator Contador()
    {
        carreraFinalizada = false;
        
        yield return new WaitForSeconds(3);
        
        while (!carreraFinalizada)
        {
            yield return new WaitForSeconds(1);
            contadorTiempo++;
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
    
    void FixedUpdate()
    {
        if (_informacionJugadores != null 
            //&& NetworkClient.localPlayer.gameObject.GetComponent<CarController>().enableControls 
            && isServer)  
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
                    if (!cuentaAtrasActivado)
                    {
                        CmdInicioCuentaAtras();
                    }
                    
                    //Cuando un jugador acaba la carrera
                    GestionCarreraTerminada(jugador, puntuacionMaxima - 2 * (jugador.posicionActual-1));

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

    [Command (requiresAuthority = false)]
    private void CmdInicioCuentaAtras()
    {
        RpcInicioCuentaAtras();
    }
    
    [ClientRpc]
    private void RpcInicioCuentaAtras()
    {
        Debug.Log("CREANDO UNA CORRUTINA");
        StartCoroutine(CuentaAtrasCarrera());
    }
    
    private IEnumerator CuentaAtrasCarrera()
    {
        cuentaAtrasActivado = true;
        
        while (segundosRestantes > 0)
        {
            foreach (var jugador in _informacionJugadores)
            {
                jugador._interfazController.CuentaAtras(cuentaAtrasActivado, segundosRestantes);
            }
            yield return new WaitForSeconds(1);
            segundosRestantes--;
        }
        
        foreach (var jugador in _informacionJugadores)
        {
            if(!jugador.finCarrera) 
                GestionCarreraTerminada(jugador, (puntuacionMaxima - 2 * (jugador.posicionActual-1))/2);
        }
                
        cuentaAtrasActivado = false;
        carreraFinalizada = true;

    }

    public void GestionCarreraTerminada(InformacionJugador jugador, int puntos)
    {
        jugador.finCarrera = true;
        jugador.CmdSetFinCarrera(true);
        _gameManager.ActualizarPuntuacionJugadorCarrera(jugador, puntos, contadorTiempo);
        TargetFinishRace(jugador,jugador.posicionActual-1);
    }
  
    public void TargetFinishRace(InformacionJugador target, int sumOrd)
    {
        target.transform.position = spawnsFinales[sumOrd].transform.position;
        target.transform.rotation = spawnsFinales[sumOrd].transform.rotation;
        target.gameObject.GetComponent<InformacionJugador>()._carController.DesactivateCar();
    }
    
}
