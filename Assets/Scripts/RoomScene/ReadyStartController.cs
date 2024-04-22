using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ReadyStartController : MonoBehaviour
{
    public bool isReady = false;
    public Button btnReady;
    
    private Image colorBtnReady;
    private string colorRojo = "#CC6666";
    private string colorVerde = "#B8DABA";

    public TextMeshProUGUI txtReady;
    
    // Start is called before the first frame update
    void Start()
    {
        colorBtnReady = btnReady.GetComponent<Image>();
        btnReady.onClick.AddListener(gestionReady);
    }

    private void gestionReady()
    {
        Color cambioColor;
        string cambioTexto;
        
        //Ajustes cuando el jugador este listo
        if (!isReady)
        {
            
            isReady = true;
            cambioColor = HexToColor(colorVerde);
            cambioTexto = "READY";
            
        }
        //Ajustes cuando el jugador no este listo
        else
        {
            isReady = false;
            cambioColor = HexToColor(colorRojo);
            cambioTexto = "NOT READY";
        }
        
        colorBtnReady.color = cambioColor;
        txtReady.text = cambioTexto;
    }
    
    private Color HexToColor(string hex)
    {
        hex = hex.Replace("#", "");
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

        return new Color32(r, g, b, 255);
    }
}
