using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;



public class GameManager : NetworkBehaviour
{
    
    public List<string> ordenCircuitos;
    public List<GameObject> ordenMinijuegos;
    public List<GameObject> playersOrder;


    [Command (requiresAuthority = false)]
    public void CheckAllPlayersWaiting()
    {
        Debug.Log("HOLAA??");
        foreach (var playerConnection in NetworkServer.connections)
        {
            if (playerConnection.Value.identity.gameObject.GetComponent<InformacionJugador>().lastMinigameScore==null)
                return;
        }
        
        Debug.Log("Vamos a camiar de escena");

        CambiaAlSiguienteJuego();
    }

    private void CambiaAlSiguienteJuego()
    {
        GetPuntuacionesM0();
    }

    [ClientRpc] 
    private void GetPuntuacionesM0()
    {
        M0GameManager m0GameManager = FindObjectOfType<M0GameManager>().GetComponent<M0GameManager>();
        Debug.Log("connectionToServer");
        // Debug.Log(connectionToServer.identity);
        // puntosJugadoresM0.Add(connectionToServer.identity.gameObject.GetInstanceID(),m0GameManager.PuntosTotales());
        Debug.Log("Puntuaciones jugadoes");
        // Debug.Log(puntosJugadoresM0);
    }
    
    
    
}
