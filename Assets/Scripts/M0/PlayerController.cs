using System.Collections;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    public float speed = 5f;
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI finalMessage;
    public GameObject messagePanel;
    public GameObject wrench;

    private int piecesCollected = 0;
    private int lastPiecesCollected = 0;
    private float lastCollectedTime; 
    public int totalPieces = 4;

    private bool puzzleCompleted = false;
    private float startTime;
    private float maxTime = 60f;
    private float tiempoRegistrado;
    
    public Rigidbody2D rb;
    private Collider2D wrenchCollider;
    private Transform wrenchTransform;
    
    public bool controlMovimiento = true;
    
    public PlayerInput _playerInput;


    void Start()
    {
        startTime = Time.time;
        rb = GetComponent<Rigidbody2D>();
        wrenchCollider = wrench.GetComponent<Collider2D>();
        wrenchTransform = wrench.transform;
        
        _playerInput = GetComponent<PlayerInput>();
        
    }

    void Update()
    {
        if (!puzzleCompleted)
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
                        gameObject.GetComponent<Animator>().SetBool("Lado", true);

                        SetWrenchRotationAndPosition(90f, new Vector2(-0.5f, -0.15f));
                    }
                    else if (movement.x > 0)
                    {
                        gameObject.GetComponent<Animator>().SetBool("Detras", false);
                        gameObject.GetComponent<Animator>().SetBool("Delante", false);
                        gameObject.GetComponent<Animator>().SetBool("Lado", true);

                        SetWrenchRotationAndPosition(-90f, new Vector2(0.5f, -0.15f));
                    }
                }
                else
                {
                    movement.x = 0f;

                    if (movement.y < 0)
                    {
                        gameObject.GetComponent<Animator>().SetBool("Detras", false);
                        gameObject.GetComponent<Animator>().SetBool("Delante", true);
                        gameObject.GetComponent<Animator>().SetBool("Lado", false);

                        SetWrenchRotationAndPosition(180f, new Vector2(-0.1f, -0.7f));
                    }
                    else if (movement.y > 0)
                    {
                        gameObject.GetComponent<Animator>().SetBool("Detras", true);
                        gameObject.GetComponent<Animator>().SetBool("Delante", false);
                        gameObject.GetComponent<Animator>().SetBool("Lado", false);

                        SetWrenchRotationAndPosition(0f, new Vector2(0.1f, 0.7f));
                    }
                }

                if (controlMovimiento)
                {
                    rb.velocity = new Vector2(movement.x * speed, movement.y * speed);
                }
            }
        }

        // Verificar si ha pasado el tiempo límite de tiempo
        float elapsedTime = Time.time - startTime;
        float remainingTime = Mathf.Max(maxTime - elapsedTime, 0f);

        // Actualizar el texto de la cuenta atrás
        if (countdownText != null)
        {
            countdownText.text = "Tiempo: " + Mathf.Ceil(remainingTime);
        }

        if (elapsedTime >= maxTime)
        {
            EndGame();
        }
        
        float clampedX = Mathf.Clamp(transform.position.x, -10.5f, 11.5f);
        float clampedY = Mathf.Clamp(transform.position.y, -8.25f, 7.25f);
        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Checkpoint") && !collision.GetComponent<CheckpointController>().IsCollected())
        {
            CollectPiece(collision.gameObject);
        }
    }
    
    private void TryBreakBox()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(wrenchCollider.bounds.center, wrenchCollider.bounds.size, 0f);

        foreach (Collider2D collider in colliders)
        {
            BoxController boxController = collider.GetComponent<BoxController>();
            if (boxController != null)
            {
                boxController.HitBox();
            }
        }
    }

    private void CollectPiece(GameObject piece)
    {
        piecesCollected++;
        lastPiecesCollected = piecesCollected;
        lastCollectedTime = Time.time; 
        Debug.Log("Pieza " + piecesCollected + " recogida");

        // Marcar la pieza como recolectada para evitar duplicados
        piece.GetComponent<CheckpointController>().Collect();

        if (piecesCollected == totalPieces)
        {
            puzzleCompleted = true;
            tiempoRegistrado = Time.time - startTime;
            EndGame();
        }
    }

    private void EndGame()
    {
        if (puzzleCompleted)
        {
            messagePanel.SetActive(true);
            gameObject.SetActive(false);
            finalMessage.text = "Minijuego completado en " + FormatTime(tiempoRegistrado) + " segundos\n¡Bien hecho!";
        }
        else
        {
            messagePanel.SetActive(true);
            gameObject.SetActive(false);
            finalMessage.text = $"Tiempo límite alcanzado\nÚltima pieza recogida en {FormatTime(lastCollectedTime)} segundos\nTotal de piezas: {lastPiecesCollected}";
        }

    }

    private string FormatTime(float time)
    {
        int seconds = Mathf.FloorToInt(time);
        int milliseconds = Mathf.FloorToInt((time - seconds) * 1000f);
        return $"{seconds}.{milliseconds:D7}";
    }
    
    private void SetWrenchRotationAndPosition(float angle, Vector2 position)
    {
        wrenchTransform.rotation = Quaternion.Euler(0f, 0f, angle);
        wrenchTransform.localPosition = position;
    }
    

    

    public IEnumerator Empujar( Vector2 normal)
    {
        controlMovimiento = false;
        rb.velocity = normal;
        
        yield return new WaitForSeconds(0.2f);
        
        controlMovimiento = true;
    }
}
