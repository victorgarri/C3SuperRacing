using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;


public class PuntosM0
{
    public int piecesCollected;
    public float tiempoRegistrado;
    public float lastCollectedTime;

    public PuntosM0(int i, float f, float lastCollectedTime1)
    {
        piecesCollected = i;
        tiempoRegistrado = f;
        lastCollectedTime = lastCollectedTime1;
    }
}

public class GameManager : NetworkBehaviour
{
    
    public List<string> ordenCircuitos;
    public List<GameObject> ordenMinijuegos;
    
    public List<GameObject> playersOrder;

    [SerializeField]
    private Dictionary<int, PuntosM0> puntosJugadoresM0;
    
    [Command (requiresAuthority = false)]
    public void CheckAllPlayersWaiting()
    {
        Debug.Log("HOLAA??");
        foreach (var playerConnection in NetworkServer.connections)
        {
            if (!playerConnection.Value.identity.gameObject.GetComponent<InformacionJugador>().waiting)
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
        puntosJugadoresM0.Add(connectionToServer.identity.gameObject.GetInstanceID(),m0GameManager.PuntosTotales());
    }
    
    
    
}
