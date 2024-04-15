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
    
    public GameObject wrench;
    private Collider2D wrenchCollider;
    private Transform wrenchTransform;
    private M0GameManager moGameManager;

    public bool disableControls = false;

    public SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        wrenchCollider = wrench.GetComponent<Collider2D>();
        wrenchTransform = wrench.transform;
        moGameManager = FindObjectOfType<M0GameManager>();
        
        _playerInput = GetComponent<PlayerInput>();

        spriteRenderer = GetComponent<SpriteRenderer>();
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
                        gameObject.GetComponent<Animator>().SetBool("Lado", true);

                        spriteRenderer.flipX = true;

                        SetWrenchRotationAndPosition(90f, new Vector2(-0.5f, -0.15f));
                    }
                    else if (movement.x > 0)
                    {
                        gameObject.GetComponent<Animator>().SetBool("Detras", false);
                        gameObject.GetComponent<Animator>().SetBool("Delante", false);
                        gameObject.GetComponent<Animator>().SetBool("Lado", true);
                        
                        spriteRenderer.flipX = false;

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
        
        float clampedX = Mathf.Clamp(transform.position.x, 489f, 511f);
        float clampedY = Mathf.Clamp(transform.position.y, 39.5f, 56.9f);
        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Checkpoint") && !collision.GetComponent<CheckpointController>().IsCollected())
        {
            moGameManager.CollectPiece(collision.gameObject);
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
        
        //gameObject.GetComponent<Animator>().SetTrigger("Attack");
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
