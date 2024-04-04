using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;


public struct PlayerPoints
{
    public NetworkConnectionToClient networkConnectionToClient;
    public int points;
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
    
    public List<PlayerPoints> lastMinigamePlayerPoints = new List<PlayerPoints>();

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
    
    private List<GameObject[]> spawnPoints;

    [SerializeField]
    private CountDownText _countDownText;

    

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
                lastMinigamePlayerPoints.Add(new PlayerPoints()
                {
                    networkConnectionToClient = playerConnection.Value,
                    points = (int)lastMinigameScore
                });
        }
        
        lastMinigamePlayerPoints.Sort( (p1,p2) => p2.points.CompareTo(p1.points));
        foreach (var playerPoints in lastMinigamePlayerPoints)
        {
            Debug.Log("Player: "+playerPoints.networkConnectionToClient);
            Debug.Log("Points: "+playerPoints.points);
        }
        
        Debug.Log("Todo el mundo está ready, vamos a camiar de escena");

        CambiaAlSiguienteJuego();
    }

    private void CambiaAlSiguienteJuego()
    {

        

        if (currentGameType==GameType.Minigame)
        {
            raceIndex++;
            ordenMinijuegos[0].SetActive(false);
            EnableCarClientRPC();
            
        }
        else
        {
            
        }

        for (int i = 0; i < lastMinigamePlayerPoints.Count; i++)
        {
            lastMinigamePlayerPoints[i].networkConnectionToClient.identity.gameObject.transform.position = spawnPoints[raceIndex][i].transform.position;
            lastMinigamePlayerPoints[i].networkConnectionToClient.identity.gameObject.transform.rotation = spawnPoints[raceIndex][i].transform.rotation;
        }
        
    }
    
    [ClientRpc]
    private void EnableCarClientRPC()
    {
        _countDownText.StartCountDown(3);
        NetworkClient.localPlayer.gameObject.GetComponent<CarController>().ActivateCar(3);
    }
    
    [ClientRpc]
    private void DisableCarClientRPC()
    {
        NetworkClient.localPlayer.gameObject.GetComponent<CarController>().DesactivateCar();
    }
    
    
    
}
