using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReadyStartController : NetworkBehaviour
{
    public bool isReady = false;
    public Button btnReady;
    public Button btnStart;
    public TMP_InputField playerNameInput;
    public Toggle spectatorMode;
    
    private Image colorBtnReady;
    private string colorRojo = "#CC6666";
    private string colorVerde = "#B8DABA";

    public TextMeshProUGUI txtReady;

    private MyNRM NRM;
    // Start is called before the first frame update
    void Start()
    {
        NRM = MyNRM.singleton as MyNRM;
        
        colorBtnReady = btnReady.GetComponent<Image>();
        btnReady.onClick.AddListener(gestionReady);
        
        if (isServer)
        {
            btnStart.gameObject.SetActive(true);
            btnStart.onClick.AddListener(delegate { NRM.StartGame(); });
        }
        
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

    private void gestionModoEspectador(bool isOn)
    {
        if (isOn)
        {
            // Debug.Log("El Toggle está activado");
            // Realiza acciones específicas cuando el Toggle está activado
        }
        else
        {
            // Debug.Log("El Toggle está desactivado");
            // Realiza acciones específicas cuando el Toggle está desactivado
        }
    }
}
