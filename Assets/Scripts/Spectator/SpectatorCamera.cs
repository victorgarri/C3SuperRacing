using Cinemachine;
using UnityEngine;

public class SpectatorCamera : MonoBehaviour
{
    private CinemachineVirtualCamera _cinemachineVirtualCamera;
    [SerializeField] private PosicionCarreraController posicionCarreraController;

    [SerializeField] private InterfazUsuarioModoEspectador _interfazUsuarioModoEspectador;
    private string nombreAnterior = "";

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
            
            _cinemachineVirtualCamera.LookAt = informacionJugador.transform;
            if (nombreAnterior != informacionJugador.nombreJugador)
            {
                _interfazUsuarioModoEspectador.CambiarNombre(informacionJugador.nombreJugador);
            }
            nombreAnterior = informacionJugador.nombreJugador;
            
            return;
        }
    }
}
