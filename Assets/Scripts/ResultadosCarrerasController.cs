using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ResultadosCarrerasController : MonoBehaviour
{
    public class listaDatos
    { 
        public string nombreJugador { get; set; } 
        public List<int> listaPuntos { get; set; }
        public int puntuacionTotal { get; set; }
        public listaDatos(string nombre, List<int> puntos)
        {
            nombreJugador = nombre;
            listaPuntos = puntos;
        }
    }

    public GameObject cuadroInfo;
    public Transform panel;
    
    public List<listaDatos> puntuacionJugadores = new List<listaDatos>();
    
    // Start is called before the first frame update
    void Start()
    {
        //Creo datos falsos
        puntuacionJugadores.Add(new listaDatos("Joche", new List<int> {1, 0, 3}));
        puntuacionJugadores.Add(new listaDatos("Pepe", new List<int> {2, 5, 0}));
        puntuacionJugadores.Add(new listaDatos("Lucía", new List<int> {2, 5, 0}));

        //Me calcula la puntuación total
        foreach (var jugador in puntuacionJugadores)
        {
            jugador.puntuacionTotal = 0;
            foreach (var puntos in jugador.listaPuntos)
            {
                jugador.puntuacionTotal += puntos;
            }
        }
        
        //Me lo ordena por puntuación
        puntuacionJugadores.Sort((a, b) => b.puntuacionTotal.CompareTo(a.puntuacionTotal));

        int orden = 1;
        foreach (var informacion in puntuacionJugadores)
        {
            GameObject cuadro = Instantiate(cuadroInfo, panel);
            rellenaCuadro(cuadro, informacion, orden);
            orden++;
        }
    }

    void rellenaCuadro(GameObject cuadro, listaDatos informacionJugador, int orden)
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
