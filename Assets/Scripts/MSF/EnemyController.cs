using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private float speed;
    private Transform target;
    private MFuerzaGameManager mFuerzaGameManager;
    public GameObject enemigoMuertoPrefab;

    void Start()
    {
        mFuerzaGameManager = FindObjectOfType<MFuerzaGameManager>();
    }
    
    void Update()
    {
        if (target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
    }

    public void SetSpeed(float newSpeed, Transform friendTransform)
    {
        speed = newSpeed;
        target = friendTransform;
    }
    
    public void DestroyEnemy()
    {
        GameObject enemigoMuerto = Instantiate(enemigoMuertoPrefab, transform.position, transform.rotation);

        mFuerzaGameManager.IncrementEnemiesDestroyed();
        mFuerzaGameManager.UpdateLastDestroyedTime();
        
        Destroy(gameObject);
    }

    public void DestroyEnemiesAlTerminarPartida()
    {
        Destroy(gameObject);
    }

}