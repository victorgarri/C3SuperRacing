using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

    public class listaDatos
    {
        public string nombreJugador { get; set; } 
        public List<int> listaPuntos { get; set; }
        public int puntuacionTotal { get; set; }

        public listaDatos(string nombre, List<int> puntos, int total)
        {
            nombreJugador = nombre;
            listaPuntos = puntos;
            puntuacionTotal = total;
        }
    }
public class ResultadosCarrerasController : MonoBehaviour
{

    public GameObject cuadroInfo;
    public Transform panel;
    
    public List<listaDatos> puntuacionJugadores = new List<listaDatos>();

    
    // private InformacionJugador[] _informacionJugadores;
    //
    // // Start is called before the first frame update
    // void Start()
    // {
    //     _informacionJugadores = FindObjectsOfType(typeof(InformacionJugador)) as InformacionJugador[];
    //     
    //     //Creo datos falsos
    //     puntuacionJugadores.Add(new listaDatos("Joche", new List<int> {10, 0, 3}, 0));
    //     puntuacionJugadores.Add(new listaDatos("Pepe", new List<int> {2, 5, 0}, 0));
    //     puntuacionJugadores.Add(new listaDatos("Lucía", new List<int> {2, 5, 0}, 0));
    //
    //     //Me calcula la puntuación total
    //     foreach (var jugador in puntuacionJugadores)
    //     {
    //         jugador.puntuacionTotal = 0;
    //         foreach (var puntos in jugador.listaPuntos)
    //         {
    //             jugador.puntuacionTotal += puntos;
    //         }
    //     }
    //     
    //     //Me lo ordena por puntuación
    //     puntuacionJugadores.Sort((a, b) => b.puntuacionTotal.CompareTo(a.puntuacionTotal));
    //
    //     int orden = 1;
    //     foreach (var informacion in puntuacionJugadores)
    //     {
    //         GameObject cuadro = Instantiate(cuadroInfo, panel);
    //         RellenaCuadro(cuadro, informacion, orden);
    //         orden++;
    //     }
    // }
    //
    //
    // // Update is called once per frame
    // void Update()
    // {
    //     
    // }

    void RellenaCuadro(GameObject cuadro, listaDatos informacionJugador, int orden)
    {
        TextMeshProUGUI textoPosicion = cuadro.transform.Find("Posicion").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI textoNombre = cuadro.transform.Find("NJugador").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI textoPuntoC1 = cuadro.transform.Find("Circuito1").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI textoPuntoC2 = cuadro.transform.Find("Circuito2").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI textoPuntoC3 = cuadro.transform.Find("Circuito3").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI textoPuntuacionTotal = cuadro.transform.Find("Total").GetComponent<TextMeshProUGUI>();
    
        textoPosicion.text = orden+"º";
        textoNombre.text = informacionJugador.nombreJugador;
        textoPuntoC1.text = informacionJugador.listaPuntos[0].ToString();
        textoPuntoC2.text = informacionJugador.listaPuntos[1].ToString();
        textoPuntoC3.text = informacionJugador.listaPuntos[2].ToString();
        textoPuntuacionTotal.text = informacionJugador.puntuacionTotal.ToString();
    }
    public void agregaTablaJugador(InformacionJugador jugador, int orden)
    {
        puntuacionJugadores.Add(new listaDatos(jugador.nombreJugador, new List<int> {jugador.listaPuntuacionCarrera[0], jugador.listaPuntuacionCarrera[1], jugador.listaPuntuacionCarrera[2]}, jugador.puntuacionTotalCarrera));
        
        GameObject cuadro = Instantiate(cuadroInfo, panel);
        RellenaCuadro(cuadro, puntuacionJugadores[orden - 1], orden);
    }

    public void actualizarTablaPuntuacion()
    {
        puntuacionJugadores.Sort((a, b) => b.puntuacionTotal.CompareTo(a.puntuacionTotal));
        
        int orden = 1;
        foreach (var informacion in puntuacionJugadores)
        {
            GameObject cuadro = Instantiate(cuadroInfo, panel);
            RellenaCuadro(cuadro, informacion, orden);
            orden++;
        }
        
    }
    public void borrarTabla()
    {
        //Borro los cuadros
        GameObject[] listaCuadro = GameObject.FindGameObjectsWithTag("CuadroInfo");
        foreach (var cuadroEliminar in listaCuadro)
        {
            Destroy(cuadroEliminar);
        }
        
        puntuacionJugadores.Clear();
        
    }
}
