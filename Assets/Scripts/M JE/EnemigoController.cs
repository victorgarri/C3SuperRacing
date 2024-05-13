using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemigoController : MonoBehaviour
{
    [Header("Efecto sonido")] 
    private AudioSource _audioSource;
    
    [Header("Sistema de puntuación")]
    [SerializeField] private MJEGameManager _gameManager;
    
    [Header("Puntuación Enemigo")] 
    [SerializeField] private int puntuacionEnemigo;

    private bool esGolpeado;
    
    void Start()
    {
        _audioSource = this.GetComponent<AudioSource>();

        esGolpeado = false;
    }
    
    private void OnCollisionEnter2D(Collision2D collision2D)
    {
        if (collision2D.gameObject.CompareTag("Bala"))
        {
            if (!esGolpeado)
            {
                esGolpeado = true;
                _gameManager.ActualizarPuntuacion(puntuacionEnemigo);
                StartCoroutine(AnimacionMuerteAlien());
            }
            
            Destroy(collision2D.gameObject);
        }
    }
    public IEnumerator AnimacionMuerteAlien()
    {
        _audioSource.Play();
        
        this.GetComponent<Animator>().SetBool("Dead", true);

        yield return new WaitForSeconds(0.5f);
        
        Destroy(this.gameObject);
    }
}
