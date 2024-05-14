using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultadosCarrerasController : MonoBehaviour
{

    public GameObject cuadroInfo;
    public Transform panel;
    [SerializeField] private GameManager _gameManager;
    
    void RellenaCuadro(GameObject cuadro, PlayerRacePoints informacionJugador, int orden)
    {
        Image colorFondo = cuadro.gameObject.GetComponent<Image>();
        //Cambia el color solo al jugador local
        if (informacionJugador.networkIdentity.isLocalPlayer)
        {
            colorFondo.color = new Color32(212, 175, 55, 255);  
        }
        
        //Pillo donde debe poner la posición y el nombre del jugador
        TextMeshProUGUI textoPosicion = cuadro.transform.Find("Posicion").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI textoNombre = cuadro.transform.Find("NJugador").GetComponent<TextMeshProUGUI>();
        
        //Pillo donde debe poner la punutación del jugador
        TextMeshProUGUI textoPuntoC1 = cuadro.transform.Find("Circuito1/Puntos1").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI textoPuntoC2 = cuadro.transform.Find("Circuito2/Puntos2").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI textoPuntoC3 = cuadro.transform.Find("Circuito3/Puntos3").GetComponent<TextMeshProUGUI>();
        
        //Pillo donde debe poner el tiempo del jugador que hace en cada carrera
        TextMeshProUGUI textoTiempoC1 = cuadro.transform.Find("Circuito1/Tiempo1").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI textoTiempoC2 = cuadro.transform.Find("Circuito2/Tiempo2").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI textoTiempoC3 = cuadro.transform.Find("Circuito3/Tiempo3").GetComponent<TextMeshProUGUI>();
        
        //Pillo donde debe poner la puntuación total
        TextMeshProUGUI textoPuntuacionTotal = cuadro.transform.Find("Total/PuntosTotal").GetComponent<TextMeshProUGUI>();
        
        //Pillo donde debo poner el tiempo total
        TextMeshProUGUI textoTiempoTotal = cuadro.transform.Find("Total/TiempoTotal").GetComponent<TextMeshProUGUI>();
    
        //Pinto la posición y el orden
        textoPosicion.text = orden+"º";
        textoNombre.text = informacionJugador.networkIdentity.gameObject.GetComponent<InformacionJugador>().nombreJugador;

        //Pinto las puntuaciones de cada carrera
        List<TextMeshProUGUI> textoListaPuntuacionPorCarrera = new List<TextMeshProUGUI>();
        textoListaPuntuacionPorCarrera.Add(textoPuntoC1);
        textoListaPuntuacionPorCarrera.Add(textoPuntoC2);
        textoListaPuntuacionPorCarrera.Add(textoPuntoC3);
        RellenaPuntuacion(textoListaPuntuacionPorCarrera, informacionJugador, "Puntos");
        
        //Pinto los tiempos de cada carrera
        List<TextMeshProUGUI> textoListaTiempoPorCarrera = new List<TextMeshProUGUI>();
        textoListaTiempoPorCarrera.Add(textoTiempoC1);
        textoListaTiempoPorCarrera.Add(textoTiempoC2);
        textoListaTiempoPorCarrera.Add(textoTiempoC3);
        RellenaPuntuacion(textoListaTiempoPorCarrera, informacionJugador, "Tiempo");
        
        //Pinto la puntuación total
        textoPuntuacionTotal.text = informacionJugador.puntuacionTotal.ToString();
        
        //Pinto el tiempo total
        textoTiempoTotal.text = informacionJugador.tiempoTotal+ " s";
    }

    private void RellenaPuntuacion(List<TextMeshProUGUI> textoLista, PlayerRacePoints informacionJugador, string valorAlmacenado)
    {
        for (int i = 0; i < textoLista.Count; i++)
        {
            if (valorAlmacenado == "Puntos")
            {
                if (informacionJugador.listaPuntuacionCarrera[i] == 0) 
                    textoLista[i].text = "-";
                else 
                    textoLista[i].text = informacionJugador.listaPuntuacionCarrera[i].ToString();
            }
            else
            {
                if (informacionJugador.listaTiempoCarrera[i] == 0) 
                    textoLista[i].text = "-";
                else 
                    textoLista[i].text = informacionJugador.listaTiempoCarrera[i] + " s";
            }
        }   
    }

    public void actualizarTablaPuntuacion()
    {
        borrarTabla();
        
        var auxPlayerPoints = _gameManager.playerRacePointsList.OrderBy(jugador => jugador.tiempoTotal)
            .OrderByDescending(jugador => jugador.puntuacionTotal);
        
        int orden = 1;
        foreach (var informacion in auxPlayerPoints)
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
    }
}
