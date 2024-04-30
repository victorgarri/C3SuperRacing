using Cinemachine;
using UnityEngine;

public class SpectatorCamera : MonoBehaviour
{
    private CinemachineVirtualCamera _cinemachineVirtualCamera;
    [SerializeField] private PosicionCarreraController posicionCarreraController;

    private uint previousTargetPlayer;

    private void Start()
    {
        _cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    private void Update()
    {
        foreach (var informacionJugador in posicionCarreraController._informacionJugadores)
        {
            if (informacionJugador.finCarrera) continue;
            _cinemachineVirtualCamera.LookAt = posicionCarreraController._informacionJugadores[0].transform;
            return;

        }
    }
}
