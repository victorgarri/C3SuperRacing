using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameStartEndMin0 : MonoBehaviour
{
    public GameObject personaje;

    private TransicionManagerMin0 _transicionManagerMin0;

    private bool acabado = false;
    
    // Start is called before the first frame update
    void Start()
    {
        _transicionManagerMin0 = GetComponent<TransicionManagerMin0>();
        _transicionManagerMin0.transicionInicio();
    }
    
    // Update is called once per frame
    void Update()
    {
        if (acabado)
        {
            _transicionManagerMin0.transicionFinal();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == personaje)
        {
            acabado = true;
        }
    }
}
