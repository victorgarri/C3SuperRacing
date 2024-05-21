using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Flecha : MonoBehaviour
{
    [SerializeField] private Transform jugador;
    private GameObject flecha;
    private float radio = 1.5f;

    private Transform enemigoCercano;

    private void Start()
    {
        flecha = this.gameObject;
    }

    void Update()
    {
        BuscaEnemigoCercano(); //Función para buscar al enemigo más cercano
        ActualizaFlecha(); // Función para actualizar rotación y posición de la flecha
    }

    void BuscaEnemigoCercano()
    {
        //Busca todos los enemigos en el juego
        GameObject[] enemigos = GameObject.FindGameObjectsWithTag("Enemigo");
        float distanciaCercana = Mathf.Infinity;

        foreach (GameObject enemigo in enemigos)
        {
            float distancia = Vector3.Distance(jugador.position, enemigo.transform.position);
            if (distancia < distanciaCercana)
            {
                distanciaCercana = distancia;
                enemigoCercano = enemigo.transform;
            }
        }
    }

    void ActualizaFlecha()
    {
        if (enemigoCercano != null)
        {
            flecha.SetActive(true);
            Vector3 direction = enemigoCercano.position - jugador.position;
            
            direction.Normalize();
            Vector3 flechaPosition = jugador.position + direction * radio + direction;
            
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            
            flecha.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            
            flecha.transform.position = flechaPosition;
        }
        else
        {
            flecha.SetActive(false);
        }
    }
}
