using UnityEngine;

public class fxDerrapes : MonoBehaviour
{
    public ParticleSystem skidMarkSystem; // Sistema de partículas para las marcas de derrape
    public float skidThresholdSpeed = 10f; // Velocidad mínima para empezar a derrapar
    public float skidThresholdAngle = 30f; // Cambio mínimo de ángulo en grados para considerar que hay derrape

    private Rigidbody parentRigidbody; // Rigidbody del objeto padre para detectar movimiento
    private Vector3 lastForward; // Para almacenar la última dirección hacia adelante del objeto

    void Start()
    {
        parentRigidbody = GetComponentInParent<Rigidbody>(); // Obtiene el Rigidbody del objeto padre
        lastForward = transform.forward; // Inicializa la última dirección hacia adelante

        if (skidMarkSystem == null) // Comprobación de seguridad para el sistema de partículas
        {
            Debug.LogError("SkidMarkController: No Particle System assigned!");
        }
    }

    void Update()
    {
        Vector3 currentForward = transform.forward;
        float speed = parentRigidbody.velocity.magnitude;
        float angleChange = Vector3.Angle(lastForward, currentForward);

        // Comprobación si el objeto está derrapando
        if (speed > skidThresholdSpeed && angleChange > skidThresholdAngle)
        {
            if (!skidMarkSystem.isPlaying)
                skidMarkSystem.Play();
        }
        else if (skidMarkSystem.isPlaying)
        {
            skidMarkSystem.Stop();
        }

        lastForward = currentForward; // Actualizar la última dirección para el próximo frame
    }
}
