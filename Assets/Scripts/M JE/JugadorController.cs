using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;

public class JugadorController : MonoBehaviour
{
    [Header("Movimientos personaje")] 
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private float velocidadPersonaje = 5.0f;
    private bool izquierda, arriba, abajo;
    
    [Header("Movimiento inicio/Transición")]
    public bool controlBloqueado = true;
    private Vector2 direccionInicial = Vector2.right;
    [SerializeField] private float velocidadPersonajeInicio = 1f;
    private float duracionMovimientoInicial = 5f;
    private Rigidbody2D rb;
    private Animator animation;

    [Header("Disparo")]
    [SerializeField] private Transform disparoSpawn;
    [SerializeField] private GameObject disparoJugador;
     private float spawnOffset = 0.5f;
    [SerializeField] private float velocidadDisparo = 10.0f;
    private bool puedoDisparar = true;
    [SerializeField] private float delayDisparo = 0.5f;
    private string direccionDisparo = "Derecha";
    private AudioSource _audioSource;
    [SerializeField] private AudioClip sonidoDisparo;

    [Header("Script GameManager")] 
    [SerializeField] private MJEGameManager _gameManager;

    [Header("Cámara")] 
    [SerializeField] private Camara _camara;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        animation = this.GetComponent<Animator>();
        _audioSource = this.GetComponent<AudioSource>();
        
        izquierda = false;
        arriba = false;
        abajo = false;

        _playerInput = this.GetComponent<PlayerInput>();
        
        StartCoroutine(Introduccion());
    }

    private IEnumerator Introduccion()
    {
        animation.SetBool("Lateral", true);
        rb.velocity = direccionInicial.normalized * velocidadPersonajeInicio;

        yield return new WaitForSeconds(duracionMovimientoInicial);
        
        rb.velocity = Vector2.zero;
        controlBloqueado = false;
        
        //Me activa el límite del mapa
        foreach (var cuadro in _gameManager.limiteMapa)
        {
            cuadro.SetActive(true);
        }
        
        //Que empiece la cuenta atrás del juego
        StartCoroutine(_gameManager.CuentaAtras());
        
        //Cambio el límite del mínimo del eje X de la cámara
        _camara.minimo = new Vector2((-25.5f-1000), _camara.minimo.y);
    }

    // Update is called once per frame
    void Update()
    {
        //Una vez que los controles estén disponibles, podremos controlar al personaje
        if (!controlBloqueado)
        {
            if (_playerInput != null && _playerInput.actions != null && _playerInput.actions["Movimiento"] != null)
            {
                Vector2 movimiento = _playerInput.actions["Movimiento"].ReadValue<Vector2>();

                //Para controlar si hacemos movimientos verticales
                if (Mathf.Abs(movimiento.x) > Mathf.Abs(movimiento.y))
                {
                    animation.SetBool("Bajando", false);
                    animation.SetBool("Lateral", true);
                    
                    movimiento.y = 0f;
                    
                    //Movimiento izquierda
                    if (movimiento.x < 0)
                    {
                        if (!izquierda)
                        {
                            transform.eulerAngles = new Vector3(transform.eulerAngles.x, 180, 0);
                            izquierda = true;
                            arriba = false;
                            abajo = false;

                            direccionDisparo = "Izquierda";
                        }
                    }
                    
                    //Movimiento derecha
                    else
                    {
                        if (izquierda || abajo || arriba)
                        {
                            transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0, 0);
                            izquierda = false;
                            arriba = false;
                            abajo = false;
                            
                            direccionDisparo = "Derecha";
                        }
                    }
                    
                //Para controlar si hacemos movimientos horizontales    
                } else if (Mathf.Abs(movimiento.x) < Mathf.Abs(movimiento.y))
                {
                    movimiento.x = 0f;

                    //Movimiento hacia abajo
                    if (movimiento.y < 0)
                    {
                        animation.SetBool("Lateral", false);
                        
                        if (!abajo)
                        {
                            //transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0, 270);
                            animation.SetBool("Bajando", true);
                            izquierda = false;
                            arriba = false;
                            abajo = true;
                            
                            direccionDisparo = "Abajo";
                        }
                    }

                    //Movimiento hacia arriba
                    else
                    {
                        if (!arriba)
                        {
                            //transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0, 90);
                            animation.SetBool("Bajando", false);
                            izquierda = false;
                            arriba = true;
                            abajo = false; 
                            
                            direccionDisparo = "Arriba";
                        }
                    }
                }
                
                //Si esta quieto el personaje
                if (movimiento.x == movimiento.y)
                {
                    animation.SetBool("Bajando", false);
                    animation.SetBool("Lateral", false);
                }
                
                //Velocidad de movimiento del personaje
                rb.velocity = new Vector2(movimiento.x * velocidadPersonaje, movimiento.y * velocidadPersonaje);
                
                //Botón de disparo
                if (_playerInput.actions["Disparo"].WasPressedThisFrame() && puedoDisparar)
                {
                    StartCoroutine(JugadorDispara());
                }
            }
        }
    }

    private IEnumerator JugadorDispara()
    {
        DisparoPoder();
        puedoDisparar = false;

        if (_audioSource != null && sonidoDisparo != null)
        {
            _audioSource.PlayOneShot(sonidoDisparo, 1);
        }
        
        yield return new WaitForSeconds(delayDisparo);
        puedoDisparar = true;
    }

    private void DisparoPoder()
    {
        Vector3 balaSpawnPosition = disparoSpawn.position + transform.up * spawnOffset;
        
        Quaternion rotation;
        switch (direccionDisparo)
        {
            case "Arriba":
                rotation = Quaternion.Euler(0, 0, 90);
                break;
            case "Abajo":
                rotation = Quaternion.Euler(0, 0, -90);
                break;
            case "Izquierda":
                rotation = Quaternion.Euler(0, 0, 180);
                break;
            default: // "derecha"
                rotation = Quaternion.Euler(0, 0, 0);
                break;
        }
        
        GameObject bala = Instantiate(disparoJugador, balaSpawnPosition, rotation);
        
        // Obtener el componente Rigidbody de la bala
        Rigidbody2D balaRigidbody = bala.GetComponent<Rigidbody2D>();
        
        balaRigidbody.velocity = bala.transform.right * velocidadDisparo;
    }
}
