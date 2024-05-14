using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

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
    
    public List<int> listaTiempoCarrera;
    public int tiempoTotal;
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

    [Header("Modo espectador")]
    [SerializeField] private GameObject spectator;
    [SerializeField] private GameObject interfazUsuarioModoEspectador;
    
    [SerializeField] private Image panelInicio;
    
    // [SerializeField]
    // private GameObject _minijuego0;


    private void Start()
    {

        if (LocalPlayerPointer.Instance.roomPlayer.isSpectator)
        {
            spectator.gameObject.SetActive(true);
            interfazUsuarioModoEspectador.gameObject.SetActive(true);
        }
        
        raceIndex = -1;
        minigameIndex = 0;
        currentGameType = GameType.Minigame;

        
        spawnPoints = new List<GameObject[]>();
        spawnPoints.Add(SPs1);
        spawnPoints.Add(SPs2);
        spawnPoints.Add(SPs3);
        
        _resultadoCarreraController.gameObject.SetActive(false);
        // DisableWaypoints();
        playerRacePointsList.Callback += OnPlayerRacePointsListUpdated;

        if(LocalPlayerPointer.Instance.roomPlayer.isSpectator)
            DisableMinigame(0);
        
        SpectatorRaceStart(0);
            
    }

    [Command (requiresAuthority = false)]
    public void CheckAllPlayersWaiting()
    {
        // Debug.Log("Comprobando si estamos ready");
        var informacionJugadores = FindObjectsOfType<InformacionJugador>();
        
        if (currentGameType == GameType.Minigame)
        {
            foreach (var informacionJugador in informacionJugadores)
            {
                // Debug.Log("jugador waiting: "+informacionJugador.finMinijuego);
                if (!informacionJugador.finMinijuego)
                {
                    // Debug.Log("TODAVIA HAY ALGUIEN QUE NO HA TERMINADO");
                    return;
                }
            }   

            foreach (var informacionJugador in informacionJugadores)
            {
                var lastMinigameScore = informacionJugador.lastMinigameScore;
                
                if (lastMinigameScore != null)
                    lastMinigamePlayerPoints.Add(new PlayerMinigamePoints(){
                        networkIdentity = informacionJugador.GetComponent<NetworkIdentity>(),
                        points = (int)lastMinigameScore
                    });
            }
        
            lastMinigamePlayerPoints.Sort( (p1,p2) => p2.points.CompareTo(p1.points));
        }
        else
        {
            foreach (var informacionJugador in informacionJugadores)
            {
                if (informacionJugador.finCarrera==false)
                    return;
            }
        }
        
        // Debug.Log("Todo el mundo ready, iniciando cuenta atras de 5s");

        StartCoroutine(CambiaAlSiguienteJuego());
    }

    private IEnumerator CambiaAlSiguienteJuego() 
    {
        yield return new WaitForSeconds(5);
        // Debug.Log("Cambiando juego AHORA");
        var playersCarController = FindObjectsOfType<CarController>();
        if (currentGameType == GameType.Race && minigameIndex + 1 < ordenMinijuegos.Count)
        {
            minigameIndex++;
            currentGameType = GameType.Minigame;
            lastMinigamePlayerPoints = new List<PlayerMinigamePoints>();
            EnableMinigameClientRPC(minigameIndex);
            SpectatorRaceStart(raceIndex+1);
            
        }
        else if(raceIndex+1 < tracksWaypoints.Count)
        {
            raceIndex++;
            DisableMinigameClientRPC(minigameIndex);
            
            if (currentGameType == GameType.Minigame)
            {
                for (int i = 0; i < lastMinigamePlayerPoints.Count; i++)
                {
                    lastMinigamePlayerPoints[i].networkIdentity.gameObject.GetComponent<CarController>().CmdMoveCar(raceIndex,i);
                }
            }
            else
            {
                foreach (var playerCarController in playersCarController)
                { 
                    playerCarController.CmdMoveCar(raceIndex, playerCarController.GetComponent<InformacionJugador>().posicionActual-1);
                }
            }
            EnableCarClientRPC(raceIndex);
            currentGameType = GameType.Race;
        }
    }
    
    [ClientRpc]
    private void EnableCarClientRPC(int index)
    {
        _resultadoCarreraController.gameObject.SetActive(false);


        int countDownTime;
        if (index == 0) countDownTime = 8;
        else countDownTime = 3;
        _countDownText.StartCountDown(countDownTime);

        if (!LocalPlayerPointer.Instance.roomPlayer.isSpectator)
        {
            LocalPlayerPointer.Instance.gamePlayerGameObject.GetComponent<CarController>().ActivateCar(countDownTime);
            StartCoroutine(DesactivarPanelTutorial());
            interfazUsuario.GetComponent<InterfazController>().cambiosMinimapa(index);            
        }

        
        for (int i = 0; i < tracksWaypoints.Count; i++)
        {
            if(i==index)
                tracksWaypoints[i].SetActive(true);
            else
                tracksWaypoints[i].SetActive(false);
        }
        
        //Esto no me cuadra que este aqui, o el bucle de dentro
        ReseteoVariablesJugadores(index);
        
    }

    private IEnumerator DesactivarPanelTutorial()
    {
        panelInicio.gameObject.SetActive(true);
        yield return new WaitForSeconds(5);
        panelInicio.gameObject.SetActive(false);
    }
    
    [ClientRpc]
    private void DisableCarClientRPC()
    {
        if (!LocalPlayerPointer.Instance.roomPlayer.isSpectator)
            LocalPlayerPointer.Instance.gamePlayerGameObject.GetComponent<CarController>().DesactivateCar();  
    }

    [ClientRpc]
    private void DisableMinigameClientRPC(int index)
    {
        DisableMinigame(index);
    }

    private void DisableMinigame(int index)
    {
        ordenMinijuegos[index].SetActive(false);
    }
    
    [ClientRpc]
    private void EnableMinigameClientRPC(int index)
    {
        EnableMinigame(index);
    }

    private void EnableMinigame(int index)
    {
        if (LocalPlayerPointer.Instance.roomPlayer.isSpectator) return;
        
        _resultadoCarreraController.gameObject.SetActive(false);
        Debug.Log(NetworkClient.localPlayer.gameObject);
        LocalPlayerPointer.Instance.gamePlayerGameObject = NetworkClient.localPlayer.gameObject;
        Debug.Log(LocalPlayerPointer.Instance.gamePlayerGameObject);
        LocalPlayerPointer.Instance.gamePlayerGameObject.GetComponent<InformacionJugador>().SetMinigameScore(null);
        LocalPlayerPointer.Instance.gamePlayerGameObject.GetComponent<InformacionJugador>().CmdSetFinMinijuego(false);
        ordenMinijuegos[index].SetActive(true);
    }

    [Command(requiresAuthority = false)]
    public void ActualizarPuntuacionJugadorCarrera(InformacionJugador jugador, int puntuacion, int tiempo)
    {
        var playerPoints = playerRacePointsList.FirstOrDefault(i => i.networkIdentity == jugador.netIdentity);
        if (playerPoints.Equals(default(PlayerRacePoints)))
        {
            playerPoints.listaPuntuacionCarrera = new List<int>(new int[3]);
            playerPoints.listaPuntuacionCarrera[raceIndex]=puntuacion;
            playerPoints.puntuacionTotal = puntuacion;
            
            playerPoints.listaTiempoCarrera = new List<int>(new int[3]);
            playerPoints.listaTiempoCarrera[raceIndex]=tiempo;
            playerPoints.tiempoTotal = tiempo;
            
            playerPoints.networkIdentity = jugador.netIdentity;
            playerRacePointsList.Add(playerPoints);
        }
        else
        {
            var playerPointsAux = playerPoints;
            
            playerPointsAux.listaPuntuacionCarrera[raceIndex]=puntuacion;
            playerPointsAux.puntuacionTotal += puntuacion;
            
            playerPointsAux.listaTiempoCarrera[raceIndex]=tiempo;
            playerPointsAux.tiempoTotal += tiempo;

            playerRacePointsList.Remove(playerPoints);
            playerRacePointsList.Add(playerPointsAux);
        }
        // Debug.Log("CHECK PLAYERS WAITING");
        CheckAllPlayersWaiting();
    }
    
    void OnPlayerRacePointsListUpdated(SyncList<PlayerRacePoints>.Operation op, int index, PlayerRacePoints oldItem, PlayerRacePoints newItem)
    {
        if(LocalPlayerPointer.Instance.roomPlayer.isSpectator) return;
        
        
        // Aqui vamos a incluir las funciones que actualizaran la pantalla de los clientes cuando se actualice la lista de datos
        if (!isLocalPlayer && LocalPlayerPointer.Instance.gamePlayerGameObject.GetComponent<InformacionJugador>().finCarrera)
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
            jugador.nVueltasCircuito = posicionCarreraController.vueltasTotales;
            jugador.finCarrera = false;

            // if (!authority) continue;
            jugador.SetVueltaActual(1);
            jugador.SetNWaypoints(0);
            jugador.SetSiguienteWaypoint(0);
            jugador.CmdSetFinCarrera(false);
            jugador._interfazController.CuentaAtras(false, 0);
        }
        
        posicionCarreraController.stopCuentaPogresiva = StartCoroutine(posicionCarreraController.CuentaPogresiva());

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

    private void SpectatorRaceStart(int index)
    {
        if(GameObject.Find("SpectatorLocations/Starts/C" + (index + 1)))
            GameObject.Find("SpectatorLocations/Starts/C" + (index + 1)).GetComponent<CinemachineVirtualCamera>().enabled = true;

        if (GameObject.Find("SpectatorLocations/POVs/C" + index))
        {
            CinemachineVirtualCamera[] camarasQueHayQueDesactivar = GameObject.Find("SpectatorLocations/POVs/C" + index).GetComponentsInChildren<CinemachineVirtualCamera>();
            foreach (var camara in camarasQueHayQueDesactivar)
            {
                camara.enabled = false;
            }
        }
    }
}
