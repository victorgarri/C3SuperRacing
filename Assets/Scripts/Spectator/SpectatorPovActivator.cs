using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class SpectatorPovActivator : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera povVirtualCamera;
    [SerializeField] private CinemachineVirtualCamera startingCamera;
    [SerializeField] private PosicionCarreraController posicionCarreraController;

    private void Start()
    {
        posicionCarreraController = GetComponentInParent<PosicionCarreraController>();
    }

    private void OnTriggerEnter(Collider other)
    {        
        if (other.transform.parent && other.transform.parent.gameObject.CompareTag("Player"))
        {
            foreach (var informacionJugador in posicionCarreraController._informacionJugadores)
            {
                if (informacionJugador.finCarrera) continue;
                if (informacionJugador.netId != other.transform.parent.GetComponent<InformacionJugador>().netId) return;
                    
                povVirtualCamera.enabled = true;
                startingCamera.enabled = false;

                var siblingsPovs = povVirtualCamera.transform.parent.GetComponentsInChildren<CinemachineVirtualCamera>();                
                foreach (var pov in siblingsPovs)
                    if (pov != povVirtualCamera)
                        pov.enabled = false;
            }
        }
    }
}
