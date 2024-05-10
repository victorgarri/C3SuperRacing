using System.Collections;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public Rigidbody2D rb;
    public bool controlMovimiento = true;
    public PlayerInput _playerInput;
    private M0GameManager moGameManager;
    public bool disableControls = false;
    public string direccion = "detras";
    public AudioClip golpeAire;
    public AudioSource llaveAudioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        moGameManager = FindObjectOfType<M0GameManager>();
        _playerInput = GetComponent<PlayerInput>();
        llaveAudioSource.clip = golpeAire;
    }

    void Update()
    {
        if (!disableControls)
        {
            if (_playerInput != null && _playerInput.actions != null && _playerInput.actions["Movimiento"] != null)
            {
                Vector2 movement = _playerInput.actions["Movimiento"].ReadValue<Vector2>();

                if (_playerInput.actions["Golpe"].WasPressedThisFrame())
                {
                    TryBreakBox();
                }

                if (Mathf.Abs(movement.x) > Mathf.Abs(movement.y))
                {
                    movement.y = 0f;

                    if (movement.x < 0)
                    {
                        gameObject.GetComponent<Animator>().SetBool("Detras", false);
                        gameObject.GetComponent<Animator>().SetBool("Delante", false);
                        gameObject.GetComponent<Animator>().SetBool("LadoDer", false);
                        gameObject.GetComponent<Animator>().SetBool("LadoIzq", true);

                        direccion = "izquierda";
                    }
                    else if (movement.x > 0)
                    {
                        gameObject.GetComponent<Animator>().SetBool("Detras", false);
                        gameObject.GetComponent<Animator>().SetBool("Delante", false);
                        gameObject.GetComponent<Animator>().SetBool("LadoDer", true);
                        gameObject.GetComponent<Animator>().SetBool("LadoIzq", false);
                        
                        direccion = "derecha";
                    }
                }
                else
                {
                    movement.x = 0f;

                    if (movement.y < 0)
                    {
                        gameObject.GetComponent<Animator>().SetBool("Detras", false);
                        gameObject.GetComponent<Animator>().SetBool("Delante", true);
                        gameObject.GetComponent<Animator>().SetBool("LadoDer", false);
                        gameObject.GetComponent<Animator>().SetBool("LadoIzq", false);

                        direccion = "delante";
                    }
                    else if (movement.y > 0)
                    {
                        gameObject.GetComponent<Animator>().SetBool("Detras", true);
                        gameObject.GetComponent<Animator>().SetBool("Delante", false);
                        gameObject.GetComponent<Animator>().SetBool("LadoDer", false);
                        gameObject.GetComponent<Animator>().SetBool("LadoIzq", false);

                        direccion = "detras";
                    }
                }

                if (controlMovimiento)
                {
                    rb.velocity = new Vector2(movement.x * speed, movement.y * speed);
                }
            }
        }
        
        float clampedX = Mathf.Clamp(transform.position.x, -10-1000, 11f-1000);
        float clampedY = Mathf.Clamp(transform.position.y, -8f, 8.5f);
        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.CompareTag("Checkpoint") && !collision.GetComponent<CheckpointController>().IsCollected())
        {
            moGameManager.CollectPiece(collision.gameObject);
        }
        
        BoxController boxController = collision.GetComponent<BoxController>();
        if (boxController != null)
        {
            boxController.HitBox();
        }
    }
    
    private void TryBreakBox()
    {
        llaveAudioSource.Play();
        
        if (direccion == "detras")
        {
            gameObject.GetComponent<Animator>().SetTrigger("AttackDetras");
        }
        else if (direccion == "delante")
        {
            gameObject.GetComponent<Animator>().SetTrigger("AttackDelante");
        }
        else if (direccion == "izquierda")
        {
            gameObject.GetComponent<Animator>().SetTrigger("AttackIzq");
        }
        else if (direccion == "derecha")
        {
            gameObject.GetComponent<Animator>().SetTrigger("AttackDer");
        }
    }

    public IEnumerator Empujar( Vector2 normal)
    {
        controlMovimiento = false;
        rb.velocity = normal;
        
        yield return new WaitForSeconds(0.2f);
        
        controlMovimiento = true;
    }
}
