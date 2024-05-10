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
        
        TextMeshProUGUI textoPosicion = cuadro.transform.Find("Posicion").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI textoNombre = cuadro.transform.Find("NJugador").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI textoPuntoC1 = cuadro.transform.Find("Circuito1").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI textoPuntoC2 = cuadro.transform.Find("Circuito2").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI textoPuntoC3 = cuadro.transform.Find("Circuito3").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI textoPuntuacionTotal = cuadro.transform.Find("Total").GetComponent<TextMeshProUGUI>();
    
        textoPosicion.text = orden+"º";
        textoNombre.text = informacionJugador.networkIdentity.gameObject.GetComponent<InformacionJugador>().nombreJugador;

        List<TextMeshProUGUI> textoListaPuntuacionPorCarrera = new List<TextMeshProUGUI>();
        textoListaPuntuacionPorCarrera.Add(textoPuntoC1);
        textoListaPuntuacionPorCarrera.Add(textoPuntoC2);
        textoListaPuntuacionPorCarrera.Add(textoPuntoC3);
        RellenaPuntuacion(textoListaPuntuacionPorCarrera, informacionJugador);
        
        textoPuntuacionTotal.text = informacionJugador.puntuacionTotal.ToString();
    }

    private void RellenaPuntuacion(List<TextMeshProUGUI> textoListaPuntuacionPorCarrera, PlayerRacePoints informacionJugador)
    {
        for (int i = 0; i < textoListaPuntuacionPorCarrera.Count; i++)
        {
            if (informacionJugador.listaPuntuacionCarrera[i] == 0) 
                textoListaPuntuacionPorCarrera[i].text = "-";
            else 
                textoListaPuntuacionPorCarrera[i].text = informacionJugador.listaPuntuacionCarrera[i].ToString();
        }   
    }

    public void actualizarTablaPuntuacion()
    {
        borrarTabla();
        
        var auxPlayerPoints = _gameManager.playerRacePointsList.OrderByDescending(jugador => jugador.puntuacionTotal).
                                                                                                 OrderBy(jugador => jugador.tiempoTotal);
        
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
