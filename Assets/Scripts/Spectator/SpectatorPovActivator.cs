using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class SpectatorPovActivator : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera povVirtualCamera;
    [SerializeField] private PosicionCarreraController posicionCarreraController;

    private void Start()
    {
        posicionCarreraController = GetComponentInParent<PosicionCarreraController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.CompareTag("Player"));
        Debug.Log(other.transform.parent.GetComponent<InformacionJugador>().netId);
        Debug.Log(posicionCarreraController._informacionJugadores[0].netId);
        
        if (other.transform.parent.gameObject.CompareTag("Player") && other.transform.parent.GetComponent<InformacionJugador>().netId == posicionCarreraController._informacionJugadores[0].netId )
        {
            Debug.Log("Activando mi POV");
            povVirtualCamera.enabled = true;
            
            Debug.Log("Desactivando otros POV");
            var siblingsPovs = povVirtualCamera.GetComponentsInParent<CinemachineVirtualCamera>();
            foreach (var pov in siblingsPovs)
            {
                if (pov != povVirtualCamera)
                    pov.enabled = false;
            }
        }
    }
}
