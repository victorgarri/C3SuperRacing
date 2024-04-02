using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;


public struct PlayerPoints
{
    public NetworkConnectionToClient networkConnectionToClient;
    public int points;
}

public class GameManager : NetworkBehaviour
{
    
    public List<string> ordenCircuitos;
    public List<GameObject> ordenMinijuegos;

    [SerializeField]
    public List<PlayerPoints> lastMinigamePlayerPoints = new List<PlayerPoints>();

    public GameObject[] playerOrder;

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
        
        
        Debug.Log("Vamos a camiar de escena");

        CambiaAlSiguienteJuego();
    }

    private void CambiaAlSiguienteJuego()
    {
        //Reordenar lista de jugadores
        lastMinigamePlayerPoints.Sort( (p1,p2) => p2.points.CompareTo(p1.points));

        foreach (var playerPoints in lastMinigamePlayerPoints)
        {
            Debug.Log("Player: "+playerPoints.networkConnectionToClient);
            Debug.Log("Points: "+playerPoints.points);
        }
        
    }
    
    
    
}
