using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisparoController : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision2D)
    {
        if (collision2D.gameObject.CompareTag("LimiteMapa") || collision2D.gameObject.CompareTag("Obstaculo"))
        {
            Destroy(this.gameObject);
        }
    }
}
