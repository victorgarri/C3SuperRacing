using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TablaPosicionModoEspectador : MonoBehaviour
{
    public GameObject cuadroNombreJugador;
    public Transform panelModoEspectador;
    
    public void actualizarTablaPosicion(InformacionJugador[] listaJugadores)
    {
        borrarTabla();
        
        int orden = 1;
        foreach (var jugador in listaJugadores)
        {
            GameObject cuadro = Instantiate(cuadroNombreJugador, panelModoEspectador);
            RellenaCuadro(cuadro, jugador.nombreJugador, orden);
            orden++;
        }
    }
    
    public void borrarTabla()
    {
        //Borro los cuadros
        GameObject[] listaCuadro = GameObject.FindGameObjectsWithTag("CuadroPosicionamiento");
        foreach (var cuadroEliminar in listaCuadro)
        {
            Destroy(cuadroEliminar);
        }
    }
    
    void RellenaCuadro(GameObject cuadro, string nombreJugador, int orden)
    {
        Image colorFondo = cuadro.GetComponent<Image>();
        ColorearFondoCuadro(colorFondo, orden);
        
        TextMeshProUGUI textoPosicion = cuadro.transform.Find("TextoPosicionTablaPosicion").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI textoNombre = cuadro.transform.Find("TextoNombreTablaPosicion").GetComponent<TextMeshProUGUI>();
        
        textoPosicion.text = orden+" -";
        textoNombre.text = nombreJugador;
    }

    void ColorearFondoCuadro(Image colorFondo, int orden)
    {
        switch (orden)
        {
            case 1:
                colorFondo.color = HexToColor("#FFD700"); //Color dorado
                break;
            case 2:
                colorFondo.color = HexToColor("#BEBEBE"); //Color plateado
                break;
            case 3:
                colorFondo.color = HexToColor("#CD7F32"); //Color bronce
                break;
            case 4:
                colorFondo.color = HexToColor("#FFFFFF"); //Color blanco
                break;
            case 5:
                colorFondo.color = HexToColor("#ffdfd4"); //Color tono rojo suave
                break;
            case 6:
                colorFondo.color = HexToColor("#ff7b5a"); //Color tono rojo medio
                break;
            case 7:
                colorFondo.color = HexToColor("#ff5232"); //Color tono rojo duro
                break;
            case 8:
                colorFondo.color = HexToColor("#ff0000"); //Color rojo puro
                break;
        }
    }
    
    Color HexToColor(string hex)
    {
        Color color = new Color();
        ColorUtility.TryParseHtmlString(hex, out color);
        return color;
    }
}
