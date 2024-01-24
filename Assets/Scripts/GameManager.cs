using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Lista de escenarios que serán los circuitos
    [Header("ESCENARIOS CIRCUITOS")]
    public List<string> escenariosCircuitos = new List<string>();
    
    //Lista de escenarios que serán los minijuegos
    [Header("ESCENARIOS MINIJUEGOS")]
    public List<string> escenariosMinijuegos = new List<string>();
    
    void Start()
    {
        GuardarListaEscenarios();
    }
    
    // Método para guardar la lista de nombres de escenarios en PlayerPrefs
    private void GuardarListaEscenarios()
    {
        DatosJuego datosJuego = DatosJuego.Instancia;

        // Guardar datos en la lista de escenarios cargados
        datosJuego.escenariosCargados = new List<string>();

        List<string> minijuegosAleatorios = escogerMinijuegosAleatorios();

        datosJuego.escenariosCargados.Add(escenariosCircuitos[0]);  //Primer circuito
        datosJuego.escenariosCargados.Add(minijuegosAleatorios[0]); //Primer minijuego
        datosJuego.escenariosCargados.Add(escenariosCircuitos[1]);  //Segundo circuito
        datosJuego.escenariosCargados.Add(minijuegosAleatorios[1]); //Segundo minijuego
        datosJuego.escenariosCargados.Add(escenariosCircuitos[2]);  //Tercer circuito
    }

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
                segundoMinijuego = Random.Range(1, nMinijuegos);
            }

            escenariosMinijuegosAleatorios.Add(escenariosMinijuegos[primerMinijuego]);
            escenariosMinijuegosAleatorios.Add(escenariosMinijuegos[segundoMinijuego]);
        }
        else
        {
            Debug.LogError("No hay suficientes escenarios de minijuegos para seleccionar.");
        }

        return escenariosMinijuegosAleatorios;
    }
    
    public void siguienteEscenario()
    {
        DatosJuego datosJuego = DatosJuego.Instancia;
        datosJuego.indice = datosJuego.indice + 1;
        
        if (datosJuego.indice - 1 < datosJuego.escenariosCargados.Count)
        {
            string nombreEscenario = datosJuego.escenariosCargados[datosJuego.indice - 1];
        
            if (SceneManager.GetSceneByName(nombreEscenario) != null)
            {
                SceneManager.LoadScene(nombreEscenario);
                Debug.Log("Escenario cargado: " + nombreEscenario);
            }
            else
            {
                Debug.LogError("El escenario '" + nombreEscenario + "' no existe");
            }
        }
        else
        {
            Debug.Log("Minijuego terminado");
        }
    }
}
