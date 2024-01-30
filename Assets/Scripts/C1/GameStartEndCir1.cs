using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameStartEndCir1 : MonoBehaviour
{
    public GameObject personaje;

    private TransicionManagerCir1 _transicionManagerCir1;

    private bool acabado = false;

    private TextMeshProUGUI textoPuntuacion;
    private GameManager gameManager;
    
    // Start is called before the first frame update
    void Start()
    {
        _transicionManagerCir1 = GetComponent<TransicionManagerCir1>();
        _transicionManagerCir1.transicionInicio();

        textoPuntuacion = GameObject.Find("TextoPuntuacion").GetComponent<TextMeshProUGUI>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        foreach (var puntuacionJugador in gameManager.puntuacionJugadores)
        {
            textoPuntuacion.text += "Jugador "+puntuacionJugador.Key+": "+puntuacionJugador.Value+"s";
        }

    }
    
    // Update is called once per frame
    void Update()
    {
        if (acabado)
        {
            _transicionManagerCir1.transicionFinal();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == personaje)
        {
            acabado = true;
        }
    }
}
