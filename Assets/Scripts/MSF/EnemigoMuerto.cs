using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemigoMuerto : MonoBehaviour
{
    public AudioClip golpeEnemigo1, golpeEnemigo2;
    public AudioSource enemyAudioSource;
    
    void Start()
    {
        StartCoroutine(DestroyEnemyAnimation());
        StartCoroutine(DestroyEnemyAudioSource());
    }
    
    IEnumerator DestroyEnemyAnimation()
    {
        yield return new WaitForSeconds(1.7f);
        
        Destroy(gameObject);
    }
    
    IEnumerator DestroyEnemyAudioSource()
    {
        int randomNumber = Random.Range(0, 2);
        AudioClip soundToPlay = randomNumber == 0 ? golpeEnemigo1 : golpeEnemigo2;
        enemyAudioSource.clip = soundToPlay;
        enemyAudioSource.Play();
        
        yield return new WaitForSeconds(1.7f);
    }
}
