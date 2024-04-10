using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using Unity.VisualScripting;
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
    
    
    public SyncList<PlayerRacePoints> playerRacePointsList = new SyncList<PlayerRacePoints>();
    
    

    [SerializeField] private int raceIndex=0;
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

    private void Start()
    {
        minigameIndex = 0;
        
        ordenMinijuegos[0].SetActive(true);
        
        currentGameType = GameType.Minigame;
        spawnPoints = new List<GameObject[]>();
        spawnPoints.Add(SPs0);
        spawnPoints.Add(SPs1);
        spawnPoints.Add(SPs2);
        spawnPoints.Add(SPs3);
        
        
        _resultadoCarreraController.gameObject.SetActive(false);
        DisableWaypoints();
        playerRacePointsList.Callback += OnPlayerRacePointsListUpdated;
    }

    private void Update()
    {
        if (playerRacePointsList.Count>0)
            Debug.Log(playerRacePointsList[0]); 
        else
            Debug.Log("Nanai");
    }


    [Command (requiresAuthority = false)]
    public void CheckAllPlayersWaiting()
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
        
        
        CambiaAlSiguienteJuego();
    }

    private void CambiaAlSiguienteJuego()
    {
        if (currentGameType==GameType.Minigame)
        {
            raceIndex++;
            DisableMinigameClientRPC(0);
            EnableCarClientRPC(raceIndex-1);
            
        }
        else
        {
            
        }
        
        for (int i = 0; i < lastMinigamePlayerPoints.Count; i++)
        {
            lastMinigamePlayerPoints[i].networkIdentity.gameObject.GetComponent<CarController>().TargetMoveCar(raceIndex,i);
        }
        
    }
    
    [ClientRpc]
    private void EnableCarClientRPC(int index)
    {
        _countDownText.StartCountDown(3); 
        NetworkClient.localPlayer.gameObject.GetComponent<CarController>().ActivateCar(3);
        tracksWaypoints[index].SetActive(true);
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
        var playerPoints = playerRacePointsList.Find(i => i.networkIdentity == jugador.netIdentity);
        
        Debug.Log("ANTES DEL ON CHANGE?????");
        Debug.Log(playerRacePointsList.Serialize());
        if (playerPoints.Equals(default(PlayerRacePoints)))
        {
            playerPoints.listaPuntuacionCarrera = new List<int>(new int[3]);
            playerPoints.listaPuntuacionCarrera[raceIndex-1]=puntuacion;
            playerPoints.puntuacionTotal = puntuacion;
            playerPoints.networkIdentity = jugador.netIdentity;
            playerRacePointsList.Add(playerPoints);
        }
        else
        {
            var playerPointsAux = playerPoints;
            playerPointsAux.listaPuntuacionCarrera[raceIndex-1]=puntuacion;
            playerPointsAux.puntuacionTotal += puntuacion;
            playerPoints = playerPointsAux;
        }
        Debug.Log("Despues On Change");
        Debug.Log(playerRacePointsList.Serialize());
    }
    
    void OnPlayerRacePointsListUpdated(SyncList<PlayerRacePoints>.Operation op, int index, PlayerRacePoints oldItem, PlayerRacePoints newItem)
    {
        Debug.Log("On Change");
        Debug.Log(playerRacePointsList.Serialize());
        // Aqui vamos a incluir las funciones que actualizaran la pantalla de los clientes cuando se actualice la lista de datos
        if (NetworkClient.localPlayer.gameObject.GetComponent<InformacionJugador>().finCarrera)
        {
            _resultadoCarreraController.gameObject.SetActive(true);
            _resultadoCarreraController.actualizarTablaPuntuacion();
        }
    }
    
    
    private void ReseteoVariablesJugadores(int posicionCarreraControllerIndex)
    {
        var posicionCarreraController = FindObjectsOfType<PosicionCarreraController>()[posicionCarreraControllerIndex];
        
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
    
}
