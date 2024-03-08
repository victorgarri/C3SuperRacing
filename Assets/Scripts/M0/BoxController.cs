using UnityEngine;
using System.Collections;

public class BoxController : MonoBehaviour
{
    public int lives;
    public GameObject explosionPrefab;
    
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
            StartCoroutine(ExplosionTnt());
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void HitBox()
    {
        lives--;

        if (lives <= 0)
        {
            BreakBox();
        }
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
                Debug.Log(normal);
            }
        }
        
        GameObject explosion = Instantiate(explosionPrefab, transform.position, transform.rotation);

        yield return new WaitForSeconds(0.5f);

        Destroy(explosion);
        Destroy(gameObject);
    }
}