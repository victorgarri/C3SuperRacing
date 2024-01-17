using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Lista de escenarios que serán seleccionados
    private List<string> escenariosCargados;
    
    // Lista de escenarios que serán los circuitos
    [Header("ESCENARIOS CIRCUITOS")]
    public List<string> escenariosCircuitos = new List<string>();
    
    //Lista de escenarios que serán los minijuegos
    [Header("ESCENARIOS MINIJUEGOS")]
    public List<string> escenariosMinijuegos = new List<string>();

    // Método para guardar la lista de nombres de escenarios en PlayerPrefs
    public void GuardarListaEscenarios()
    {
        escenariosCargados = [esce]
        
        
        string listaCircuitos = JsonUtility.ToJson(escenariosCircuitos);
        string listaMinijuegos = JsonUtility.ToJson(escenariosMinijuegos);

        // Guardar la cadena JSON en PlayerPrefs
        PlayerPrefs.SetString("ListaEscenarios", );
        PlayerPrefs.Save();

        Debug.Log("Lista de escenarios guardada");
    }

    // Método para cargar la lista de nombres de escenarios desde PlayerPrefs
    public void CargarListaEscenarios()
    {
        // Obtener la cadena JSON de PlayerPrefs
        string listaJson = PlayerPrefs.GetString("ListaEscenarios", "");

        // Si hay una cadena JSON válida, convertirla a la lista
        if (!string.IsNullOrEmpty(listaJson))
        {
            Debug.Log("Lista de escenarios cargada");
        }
        else
        {
            Debug.Log("No se encontró una lista de escenarios guardada");
        }
    }

    // Método para cargar un escenario por su nombre
    public void CargarEscenarioPorNombre(string nombreEscenario)
    {
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
}
