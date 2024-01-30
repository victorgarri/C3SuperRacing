using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : NetworkBehaviour
{
    // Lista de escenarios que serán los circuitos
    [Header("ESCENARIOS CIRCUITOS")]
    public List<string> escenariosCircuitos = new List<string>();
    
    //Lista de escenarios que serán los minijuegos
    [Header("ESCENARIOS MINIJUEGOS")]
    public List<string> escenariosMinijuegos = new List<string>();
    
    //Lista de escenarios que van a salir durante el juego
    public List<string> escenariosCargados;
    private int indice = 0;
    
    //Variable que me guarde la puntuación de cada jugador
    [SyncVar]
    public SyncDictionary<string, float> puntuacionJugadores = new SyncDictionary<string, float>();
    
    private static GameManager _instance;

    //Hago que me guarde los escenarios y empiece por el escenario deseado antes de que cargue todo el código.
    //Además de que cuando cambie de escena no se me destruya el Game Manager, o sino se pierde los datos establecidos
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.LoadScene("Minijuego 0");
            GuardarListaEscenarios();

            // Realiza la inicialización aquí
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Método para guardar la lista de nombres de los escenarios
    private void GuardarListaEscenarios()
    {
        List<string> minijuegosAleatorios = escogerMinijuegosAleatorios();

        escenariosCargados.Add(escenariosCircuitos[0]);  //Primer circuito
        escenariosCargados.Add(minijuegosAleatorios[0]); //Primer minijuego
        escenariosCargados.Add(escenariosCircuitos[1]);  //Segundo circuito
        escenariosCargados.Add(minijuegosAleatorios[1]); //Segundo minijuego
        escenariosCargados.Add(escenariosCircuitos[2]);  //Tercer circuito
    }

    //Función para que me escoja minijuegos aleatorios
    private List<string> escogerMinijuegosAleatorios()
    {
        //Creo una lista nueva
        List<string> escenariosMinijuegosAleatorios = new List<string>();
        
        //Pillo el número de minijuegos que hay
        int nMinijuegos = escenariosMinijuegos.Count;

        if (nMinijuegos >= 2)
        {
            //Escogemos los minijuegos aleatorios
            int primerMinijuego = Random.Range(0, nMinijuegos);
            int segundoMinijuego = Random.Range(0, nMinijuegos);

            // Evitamos que salga el mismo minijuego
            while (segundoMinijuego == primerMinijuego)
            {
                segundoMinijuego = Random.Range(0, nMinijuegos);
            }

            //Se lo añadimos a la lista nueva
            escenariosMinijuegosAleatorios.Add(escenariosMinijuegos[primerMinijuego]);
            escenariosMinijuegosAleatorios.Add(escenariosMinijuegos[segundoMinijuego]);
        }
        else
        {
            Debug.LogError("No hay suficientes escenarios de minijuegos para seleccionar.");
        }

        //Devolvemos la lista
        return escenariosMinijuegosAleatorios;
    }
    
    //Método para realizar un cambio de escenario
    public void siguienteEscenario()
    {
        
        if (indice < escenariosCargados.Count)
        {
            string nombreEscenario = escenariosCargados[indice];
        
            if (SceneManager.GetSceneByName(nombreEscenario) != null)
            {
                SceneManager.LoadScene(nombreEscenario);
                Debug.Log("Escenario cargado: " + nombreEscenario);
            }
            else
            {
                Debug.LogError("El escenario '" + nombreEscenario + "' no existe");
            }
            indice = indice + 1;
        }
        else
        {
            Debug.Log("Juego terminado");
        }
    }
}
