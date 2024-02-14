using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CamaraSeleccionCoche : MonoBehaviour
{
    //Límite del desplazamiento de la cámara
    private float limiteIzquierdo = -15.0f, limiteDerecho = -5.0f;
    
    //Declaro los botones
    public Button btnIzquierda, btnDerecha;

    // Start is called before the first frame update
    void Start()
    {
        btnIzquierda.onClick.AddListener(movimientoIzquierda);
        btnDerecha.onClick.AddListener(movimientoDerecha);
    }

    // Método para mover la cámara hacia la izquierda
    public void movimientoIzquierda()
    {
        float nuevaPosicion = this.transform.position.x - 10f;
        
        if (nuevaPosicion < limiteIzquierdo)
        {
            nuevaPosicion = limiteDerecho;
        }

        this.transform.position = new Vector3(nuevaPosicion,this.transform.position.y, this.transform.position.z);
    }

    // Método para mover la cámara hacia la derecha
    public void movimientoDerecha()
    {
        float nuevaPosicion = this.transform.position.x + 10f;
        
        if (nuevaPosicion > limiteDerecho)
        {
            nuevaPosicion = limiteIzquierdo;
        }

        this.transform.position = new Vector3(nuevaPosicion,this.transform.position.y, this.transform.position.z);
    }
}
