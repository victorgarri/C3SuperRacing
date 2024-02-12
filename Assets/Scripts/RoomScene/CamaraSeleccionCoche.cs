using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CamaraSeleccionCoche : MonoBehaviour
{
    private GameObject camaraSeleccionCoche;
    private Transform posicionSiguienteVista;
    private float limiteIzquierdo = -15f, limiteDerecho = -5f;
    private float velocidadCamara = 1f;

    public Button botonIzquierda, botonDerecha;

    // Start is called before the first frame update
    void Start()
    {
        camaraSeleccionCoche = this.gameObject;
        posicionSiguienteVista = camaraSeleccionCoche.transform;
        
        botonIzquierda.onClick.AddListener(movimientoIzquierda);
        botonDerecha.onClick.AddListener(movimientoDerecha);
    }

    // Método para mover la cámara hacia la izquierda
    public void movimientoIzquierda()
    {
        posicionSiguienteVista.position = new Vector3(posicionSiguienteVista.transform.position.x - 10f, 0, 0);
        
        // Si la cámara se encuentra más allá del límite izquierdo, moverla al lado derecho
        if (posicionSiguienteVista.transform.position.x < limiteIzquierdo)
        {
            posicionSiguienteVista.position = new Vector3(limiteDerecho, 0, 0);
            camaraSeleccionCoche.transform.position = new Vector3(limiteDerecho + 10f, 0, 0);
        }
        
        camaraSeleccionCoche.transform.position = Vector3.Lerp(camaraSeleccionCoche.transform.position, posicionSiguienteVista.position, Time.deltaTime * velocidadCamara);
    }

    // Método para mover la cámara hacia la derecha
    public void movimientoDerecha()
    {
        // Si la cámara se encuentra más allá del límite derecho, moverla al lado izquierdo
        if (camaraSeleccionCoche.transform.position.x > limiteDerecho)
        {
            posicionSiguienteVista.position = new Vector3(limiteIzquierdo - 10f, 0, 0);
        }
        else // De lo contrario, moverla al límite derecho
        {
            posicionSiguienteVista.position = new Vector3(limiteDerecho, 0, 0);
        }
        
        // Interpola la posición actual de la cámara hacia la posición siguiente
        camaraSeleccionCoche.transform.position = Vector3.Lerp(camaraSeleccionCoche.transform.position, posicionSiguienteVista.position, Time.deltaTime * velocidadCamara);
    }
}
