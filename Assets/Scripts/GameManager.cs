using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

public struct PlayerMinigamePoints
{
    public NetworkIdentity networkIdentity;
    public int points;
}

public struct PlayerRacePoints
{
    public NetworkIdentity networkIdentity;
    public List<int> listaPuntuacionCarrera;
    public int puntuacionTotal;
}

public class GameManager : NetworkBehaviour
{
    public GameObject interfazUsuario;
    
    public enum GameType
    {
        Minigame,
        Race
    }
    public GameType currentGameType;
    
    public List<string> ordenCircuitos;
    public List<GameObject> ordenMinijuegos;
    
    public List<PlayerMinigamePoints> lastMinigamePlayerPoints = new List<PlayerMinigamePoints>();
    
    public readonly SyncList<PlayerRacePoints> playerRacePointsList = new SyncList<PlayerRacePoints>();

    [SerializeField] private int raceIndex;
    [SerializeField] private int minigameIndex;
    
    [SerializeField]
    private GameObject[] SPs0;
    [SerializeField]
    private GameObject[] SPs1;
    [SerializeField]
    private GameObject[] SPs2;
    [SerializeField]
    private GameObject[] SPs3;
    
    public List<GameObject[]> spawnPoints;

    [SerializeField]
    private List<GameObject> tracksWaypoints;

    [SerializeField]
    private CountDownText _countDownText;

    [SerializeField]
    private ResultadosCarrerasController _resultadoCarreraController;

    [SerializeField] private List<GameObject> roomLights;

    // [SerializeField]
    // private GameObject _minijuego0;

    private void Start()
    {
        raceIndex = -1;
        minigameIndex = 0;
        
        ordenMinijuegos[0].SetActive(true);
        
        currentGameType = GameType.Minigame;
        spawnPoints = new List<GameObject[]>();
        spawnPoints.Add(SPs1);
        spawnPoints.Add(SPs2);
        spawnPoints.Add(SPs3);
        
        _resultadoCarreraController.gameObject.SetActive(false);
        // DisableWaypoints();
        playerRacePointsList.Callback += OnPlayerRacePointsListUpdated;
    }

    [Command (requiresAuthority = false)]
    public void CheckAllPlayersWaiting()
    {
        Debug.Log("Comprobando si estamos ready");
        if (currentGameType == GameType.Minigame)
        {
            foreach (var playerConnection in NetworkServer.connections)
            {
                if (playerConnection.Value.identity.gameObject.GetComponent<InformacionJugador>().lastMinigameScore == null)
                    return;
            }

            foreach (var playerConnection in NetworkServer.connections)
            {
                var lastMinigameScore = playerConnection.Value.identity.gameObject.GetComponent<InformacionJugador>().lastMinigameScore;
                if (lastMinigameScore != null)
                    lastMinigamePlayerPoints.Add(new PlayerMinigamePoints()
                    {
                        networkIdentity = playerConnection.Value.identity,
                        points = (int)lastMinigameScore
                    });
            }
        
            lastMinigamePlayerPoints.Sort( (p1,p2) => p2.points.CompareTo(p1.points));
        }
        else
        {
            foreach (var playerConnection in NetworkServer.connections)
            {
                if (playerConnection.Value.identity.gameObject.GetComponent<InformacionJugador>().finCarrera==false)
                    return;
            }
        }
        Debug.Log("Todo el mundo ready, iniciando cuenta atras de 5s");

        StartCoroutine(CambiaAlSiguienteJuego());
        CambiaAlSiguienteJuego();
    }

    private IEnumerator CambiaAlSiguienteJuego()
    {
        yield return new WaitForSeconds(5);
        Debug.Log("Cambiando juego AHORA");
        if (currentGameType == GameType.Race && minigameIndex + 1 < ordenMinijuegos.Count)
        {
            // Activar minijuego
        }
        else if(raceIndex+1 < tracksWaypoints.Count)
        {
            
            raceIndex++;
            DisableMinigameClientRPC(0);
            
            if (currentGameType == GameType.Minigame)
            {
                for (int i = 0; i < lastMinigamePlayerPoints.Count; i++)
                {
                   
                    lastMinigamePlayerPoints[i].networkIdentity.gameObject.GetComponent<CarController>().TargetMoveCar(raceIndex,i);
                }
            }
            else
            {
                foreach (var playerConnection in NetworkServer.connections)
                { 
                    playerConnection.Value.identity.gameObject.GetComponent<CarController>().TargetMoveCar(raceIndex, playerConnection.Value.identity.gameObject.GetComponent<InformacionJugador>().posicionActual-1);
                    //apagar luces habitacion si raceIndex==1
                    // if (raceIndex==1)
                    // {
                    //     playerConnection.Value.identity.gameObject.GetComponent<CarController>().SetCarLights(true);
                    // }
                    // else
                    // {
                    //     playerConnection.Value.identity.gameObject.GetComponent<CarController>().SetCarLights(false);
                    //     
                    // }
                    
                }
                // if (raceIndex==1)
                // {
                //     SetRoomLights(false);
                //         
                // }
                // else
                // {
                //     SetRoomLights(true);
                // }
            }
            EnableCarClientRPC(raceIndex);
            currentGameType = GameType.Race;
           
        }
    }
    
    [ClientRpc]
    private void EnableCarClientRPC(int index)
    {
        _resultadoCarreraController.gameObject.SetActive(false);
        _countDownText.StartCountDown(3); 
        NetworkClient.localPlayer.gameObject.GetComponent<CarController>().ActivateCar(3);
        
        interfazUsuario.GetComponent<InterfazController>().cambiosMinimapa(index);
        
        for (int i = 0; i < tracksWaypoints.Count; i++)
        {
            if(i==index)
                tracksWaypoints[i].SetActive(true);
            else
                tracksWaypoints[i].SetActive(false);
        }
        
        ReseteoVariablesJugadores(index);
    }
    
    [ClientRpc]
    private void DisableCarClientRPC()
    {
        NetworkClient.localPlayer.gameObject.GetComponent<CarController>().DesactivateCar();
    }

    [ClientRpc]
    private void DisableMinigameClientRPC(int index)
    {
        ordenMinijuegos[index].SetActive(false);
    }

    [Command(requiresAuthority = false)]
    public void ActualizarPuntuacionJugadorCarrera(InformacionJugador jugador, int puntuacion)
    {
        var playerPoints = playerRacePointsList.FirstOrDefault(i => i.networkIdentity == jugador.netIdentity);
        
        if (playerPoints.Equals(default(PlayerRacePoints)))
        {
            playerPoints.listaPuntuacionCarrera = new List<int>(new int[3]);
            playerPoints.listaPuntuacionCarrera[raceIndex]=puntuacion;
            playerPoints.puntuacionTotal = puntuacion;
            playerPoints.networkIdentity = jugador.netIdentity;
            playerRacePointsList.Add(playerPoints);
        }
        else
        {
            var playerPointsAux = playerPoints;
            playerPointsAux.listaPuntuacionCarrera[raceIndex]=puntuacion;
            playerPointsAux.puntuacionTotal += puntuacion;

            playerRacePointsList.Remove(playerPoints);
            playerRacePointsList.Add(playerPointsAux);
        }
        CheckAllPlayersWaiting();
    }
    
    void OnPlayerRacePointsListUpdated(SyncList<PlayerRacePoints>.Operation op, int index, PlayerRacePoints oldItem, PlayerRacePoints newItem)
    {
        // Aqui vamos a incluir las funciones que actualizaran la pantalla de los clientes cuando se actualice la lista de datos
        if (NetworkClient.localPlayer.gameObject.GetComponent<InformacionJugador>().finCarrera)
        {
            _resultadoCarreraController.gameObject.SetActive(true);
            _resultadoCarreraController.actualizarTablaPuntuacion();
        }
    }
    
    private void ReseteoVariablesJugadores(int posicionCarreraControllerIndex)
    {
        var posicionCarreraController = tracksWaypoints[posicionCarreraControllerIndex].GetComponent<PosicionCarreraController>();
        
        posicionCarreraController._informacionJugadores = FindObjectsOfType(typeof(InformacionJugador)) as InformacionJugador[];
        
        foreach (var jugador in posicionCarreraController._informacionJugadores)
        {
            jugador._posicionCarreraController = posicionCarreraController;
            jugador.indiceCarrera++;
            jugador.SetVueltaActual(1);
            jugador.nVueltasCircuito = posicionCarreraController.vueltasTotales;
            jugador.SetNWaypoints(0);
            jugador.SetSiguienteWaypoint(0);
            jugador.finCarrera = false;
            jugador.CmdSetFinCarrera(false);
            jugador._interfazController.CuentaAtras(false, 0);
        }
        
        posicionCarreraController.puntuacionMaxima = 2 * posicionCarreraController._informacionJugadores.Length;
        
    }

    private void DisableWaypoints()
    {
        foreach (var trackWaypoint in tracksWaypoints)
        {
            trackWaypoint.SetActive(false);
        }
    }

    [ClientRpc]
    private void SetRoomLights(bool status)
    {
        foreach (var roomLight in roomLights)
        {
            roomLight.SetActive(status);
        }
    }
}
