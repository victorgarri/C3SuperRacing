using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class PlayerControllerSF : MonoBehaviour
{
    public float moveSpeed = 15f;
    public float minX = -32f;
    public float maxX = 24f;
    public float minY = -12f;
    public float maxY = 24f;
    public float attackDistance = 1f;
    public GameObject attackPrefab;
    private Rigidbody2D rb;
    public PlayerInput _playerInput;
    public bool controlMovimiento = true;
    public AudioClip golpeAire;
    public AudioSource punioAudioSource;
    public bool disableControls = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        _playerInput = GetComponent<PlayerInput>();
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
                    AttackClosestEnemy();
                }

                if (controlMovimiento)
                {
                    rb.velocity = new Vector2(movement.x * moveSpeed, movement.y * moveSpeed);
                }
            }
        }

        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, minY, maxY);
        transform.position = clampedPosition;
    }

    private void AttackClosestEnemy()
    {
        punioAudioSource.clip = golpeAire;
        punioAudioSource.Play();
        
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackDistance, LayerMask.GetMask("Enemy"));

        foreach (Collider2D collider in colliders)
        {
            EnemyController enemyController = collider.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                enemyController.DestroyEnemy();
            }
        }
        
        GameObject attack = Instantiate(attackPrefab, transform.position, Quaternion.identity);

        Destroy(attack, 0.1f);
    }
}