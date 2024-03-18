using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetaController : MonoBehaviour
{

    [SerializeField] private Transform[] nextSpawns;


    private void OnTriggerEnter(Collider other)
    {
        GameObject player = other.gameObject.transform.parent.gameObject;
        if (player.CompareTag("Player"))
        {
            InformacionJugador infoPlayer = player.GetComponent<InformacionJugador>();
            infoPlayer.vueltas += 1;
            if (infoPlayer.vueltas >= 6)
            {
                infoPlayer.vueltas = 0;
                player.transform.position = nextSpawns[0].transform.position;
                player.transform.rotation = nextSpawns[0].transform.rotation;
            }
        }
    }
}
