using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameStartEndMin0 : NetworkBehaviour
{
    public GameObject personaje;

    private TransicionManagerMin0 _transicionManagerMin0;

    private GameManager _gameManager;

    private float inicio;
    private float tiempoPasado;
    
    

    private bool acabado = false;
    
    // Start is called before the first frame update
    void Start()
    {
        _transicionManagerMin0 = GetComponent<TransicionManagerMin0>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        
        _transicionManagerMin0.transicionInicio();
        inicio = Time.time;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (acabado)
        {
            //En vez de 1, sería la ID del jugador
            // _gameManager.puntuacionJugadores["1"] = tiempoPasado;
            _transicionManagerMin0.transicionFinal();
            
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            acabado = true;
            personaje = other.gameObject;
            tiempoPasado = Time.time - inicio - 5f;
        }
    }

    
}
