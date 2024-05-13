using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProyectilController : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
            //FALTA IMPLEMENTAR QUÉ LE PASA AL PLAYER CUANDO LE DA UN PROYECTIL
        }
    }
}