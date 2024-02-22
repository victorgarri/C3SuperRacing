using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InformacionJugador : MonoBehaviour
{
    public string nombreJugador;

    public int posicionJugador;
    public int circuitosCompletados;
    public int vueltaActualJugador;
    public int puntoControlJugador;

    private PosicionCarreraController _posicionCarreraController;
    // Start is called before the first frame update
    void Start()
    {
        _posicionCarreraController = FindObjectOfType<PosicionCarreraController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider collision)
    {
        throw new NotImplementedException();
    }
}
