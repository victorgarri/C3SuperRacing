using System.Linq;
using TMPro;
using UnityEngine;

public class ResultadosCarrerasController : MonoBehaviour
{

    public GameObject cuadroInfo;
    public Transform panel;
    [SerializeField] private GameManager _gameManager;
    
    void RellenaCuadro(GameObject cuadro, PlayerRacePoints informacionJugador, int orden)
    {
        TextMeshProUGUI textoPosicion = cuadro.transform.Find("Posicion").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI textoNombre = cuadro.transform.Find("NJugador").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI textoPuntoC1 = cuadro.transform.Find("Circuito1").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI textoPuntoC2 = cuadro.transform.Find("Circuito2").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI textoPuntoC3 = cuadro.transform.Find("Circuito3").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI textoPuntuacionTotal = cuadro.transform.Find("Total").GetComponent<TextMeshProUGUI>();
    
        textoPosicion.text = orden+"º";
        textoNombre.text = informacionJugador.networkIdentity.gameObject.GetComponent<InformacionJugador>().nombreJugador+" "+ informacionJugador.networkIdentity.netId;
        textoPuntoC1.text = informacionJugador.listaPuntuacionCarrera[0].ToString();
        textoPuntoC2.text = informacionJugador.listaPuntuacionCarrera[1].ToString();
        textoPuntoC3.text = informacionJugador.listaPuntuacionCarrera[2].ToString();
        textoPuntuacionTotal.text = informacionJugador.puntuacionTotal.ToString();
    }

    public void actualizarTablaPuntuacion()
    {
        borrarTabla();
        
        var auxPlayerPoints = _gameManager.playerRacePointsList.OrderByDescending(jugador => jugador.puntuacionTotal);
        
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
