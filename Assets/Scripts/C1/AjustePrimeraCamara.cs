using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AjustePrimeraCamara : MonoBehaviour
{
    public Camera mainCamera;
        public float nuevoTamaño = 5f;
    
        void Start()
        {
            // Ajustar el tamaño de la cámara al inicio del juego
            mainCamera.orthographicSize = nuevoTamaño;
        }
        void Update()
        {
            
        }
}
