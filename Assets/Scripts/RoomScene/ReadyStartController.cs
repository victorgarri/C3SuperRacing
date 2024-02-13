using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ReadyStartController : MonoBehaviour
{
    private bool isReady = false;
    public Button btnReady;
    
    private Image colorBtnReady;
    private string colorRojo = "#CC6666";
    private string colorVerde = "#B8DABA";
    
    // Start is called before the first frame update
    void Start()
    {
        colorBtnReady = btnReady.GetComponent<Image>();
        btnReady.onClick.AddListener(gestionReady);
    }

    private void gestionReady()
    {
        Color cambioColor;
        if (!isReady)
        {
            isReady = true;
            cambioColor = HexToColor(colorVerde);
        }
        else
        {
            isReady = false;
            cambioColor = HexToColor(colorRojo);
        }
        
        colorBtnReady.color = cambioColor;
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
