using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendsController : MonoBehaviour
{
    public float minSpeed = 1f;
    public float maxSpeed = 3f;
    public float chanceToStop = 0.5f;
    public float chanceToMoveChunk = 0.2f;
    public float chunkDistance = 2f;
    public int damageReceived;
    private float speed;
    private Vector3 targetPosition;
    public AudioClip amigosGolpeados;
    public AudioSource amigosAudioSource;

    void Start()
    {
        speed = Random.Range(minSpeed, maxSpeed);
        targetPosition = GetRandomPosition();
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            float randomValue = Random.value;
            if (randomValue < chanceToStop)
            {
                speed = 0f;
            }
            else if (randomValue < chanceToStop + chanceToMoveChunk)
            {
                MoveChunk();
            }
            else
            {
                targetPosition = GetRandomPosition();
                speed = Random.Range(minSpeed, maxSpeed);
            }
        }
    }

    private Vector3 GetRandomPosition()
    {
        float x = Random.Range(-13f-1000, 9f-1000);
        float y = Random.Range(-2f, 9f);
        return new Vector3(x, y, transform.position.z);
    }

    private void MoveChunk()
    {
        Vector3 direction = Random.insideUnitCircle.normalized;
        targetPosition += direction * chunkDistance;
        speed = Random.Range(minSpeed, maxSpeed);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            amigosAudioSource.clip = amigosGolpeados;
            amigosAudioSource.Play();
            
            Destroy(other.gameObject);
            damageReceived++;
            
            gameObject.GetComponent<Animator>().SetTrigger("AmigosGolpeados");
        }
    }
}
