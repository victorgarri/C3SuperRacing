using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class BoxController : MonoBehaviour
{
    public int lives;
    public GameObject explosionPrefab;
    public Sprite cajaReforzadaSprite;
    public Sprite cajaSprite;
    public Sprite cajaRotaSprite;
    private SpriteRenderer spriteRenderer;
    private Transform boxTransform;
    private Vector3 originalPosition;
    private bool isMoving = false;
    private float moveDistance = 0.05f;
    private float moveDuration = 0.1f;
    private float elapsedTime = 0f;
    public AudioClip golpeCaja, golpeTnt;
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxTransform = transform;
        originalPosition = boxTransform.position;
    }
    
    public void SetBoxType(string boxType)
    {
        switch (boxType)
        {
            case "CajaReforzada":
                lives = 3;
                break;
            case "Caja":
                lives = 2;
                break;
            case "Tnt":
                lives = 1;
                break;
            default:
                lives = 1;
                break;
        }
    }
    
    public void BreakBox()
    {
        if (gameObject.CompareTag("Tnt"))
        {
            AudioSource.PlayClipAtPoint(golpeTnt, transform.position, 50);
            
            StartCoroutine(ExplosionTnt());
        }
        else
        {
            AudioSource.PlayClipAtPoint(golpeCaja, transform.position, 50);
            
            Destroy(gameObject);
        }
    }
    
    public void HitBox()
    {
        AudioSource.PlayClipAtPoint(golpeCaja, transform.position, 50);
        
        lives--;

        if (lives <= 0)
        {
            BreakBox();
        }
        else
        {
            UpdateBoxSprite();
            if (!isMoving)
            {
                StartCoroutine(MoveBox());
            }
        }
    }
    
    private void UpdateBoxSprite()
    {
        switch (lives)
        {
            case 3:
                spriteRenderer.sprite = cajaReforzadaSprite;
                break;
            case 2:
                spriteRenderer.sprite = cajaSprite;
                break;
            case 1:
                spriteRenderer.sprite = cajaRotaSprite;
                break;
            default:
                break;
        }
    }
    
    private IEnumerator MoveBox()
    {
        isMoving = true;
        while (elapsedTime < moveDuration)
        {
            boxTransform.position = originalPosition + new Vector3(Random.Range(-1f, 1f) * moveDistance, Random.Range(-1f, 1f) * moveDistance, 0);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        elapsedTime = 0f;
        boxTransform.position = originalPosition;
        isMoving = false;
    }
    
    IEnumerator ExplosionTnt()
    {
        BoxCollider2D collider2D = GetComponent<BoxCollider2D>();
        collider2D.enabled = false;
        
        Collider2D[] surroundingColliders = Physics2D.OverlapCircleAll(this.transform.position, 1.5f);

        foreach (Collider2D surroundingCollider in surroundingColliders)
        {
            BoxController boxController = surroundingCollider.GetComponent<BoxController>();
            
            if (boxController != null)
            {
                boxController.BreakBox();
            }
            
            PlayerController playerController = surroundingCollider.GetComponent<PlayerController>();
            if (playerController != null)
            {
                Vector2 normal = (playerController.transform.position - this.transform.position).normalized;
                StartCoroutine(playerController.Empujar(normal*5));
            }
        }
        
        GameObject explosion = Instantiate(explosionPrefab, transform.position, transform.rotation);

        yield return new WaitForSeconds(0.5f);

        Destroy(explosion);
        Destroy(gameObject);
    }

}