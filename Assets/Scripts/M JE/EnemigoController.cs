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
    
    void Start()
    {
        _audioSource = this.GetComponent<AudioSource>();
    }
    
    private void OnCollisionEnter2D(Collision2D collision2D)
    {
        if (collision2D.gameObject.CompareTag("Bala"))
        {
            _gameManager.ActualizarPuntuacion(puntuacionEnemigo);

            StartCoroutine(AnimacionMuerteAlien(collision2D.gameObject));
        }
    }
    public IEnumerator AnimacionMuerteAlien(GameObject bala)
    {
        _audioSource.Play();
        
        Destroy(bala);
        this.GetComponent<Animator>().SetBool("Dead", true);

        yield return new WaitForSeconds(0.5f);
        
        Destroy(this.gameObject);
    }
}
