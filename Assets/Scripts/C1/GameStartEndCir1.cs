using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameStartEndCir1 : MonoBehaviour
{
    public GameObject personaje;

    private TransicionManagerCir1 _transicionManagerCir1;

    private bool acabado = false;
    
    // Start is called before the first frame update
    void Start()
    {
        _transicionManagerCir1 = GetComponent<TransicionManagerCir1>();
        _transicionManagerCir1.transicionInicio();
    }
    
    // Update is called once per frame
    void Update()
    {
        if (acabado)
        {
            _transicionManagerCir1.transicionFinal();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == personaje)
        {
            acabado = true;
        }
    }
}
