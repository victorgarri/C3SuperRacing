using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public FriendsController friendsController;
    public float minX = -32f;
    public float maxX = 24f;
    public float minY = -12f;
    public float maxY = 24f;
    public float minSpeed = 5f;
    public float maxSpeed = 10f;
    public int enemiesSpawned = 0;
    private float startTime;
    private const int totalEnemiesToSpawn = 25;

    void Start()
    {
        friendsController = FindObjectOfType<FriendsController>();
        startTime = Time.time;
        SpawnEnemy();
    }

    void Update()
    {
        if (enemiesSpawned < totalEnemiesToSpawn && Time.time - startTime >= enemiesSpawned * 2f)
        {
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        Vector3 spawnPosition = GetRandomSpawnPosition();
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        float speed = Random.Range(minSpeed, maxSpeed);
        enemy.GetComponent<EnemyController>().SetSpeed(speed, friendsController.transform);

        enemiesSpawned++;
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector3 spawnPosition;
        do
        {
            float randomX = Random.Range(minX - 1f, maxX + 1f);
            float randomY = Random.Range(minY - 1f, maxY + 1f);
            spawnPosition = new Vector3(randomX, randomY, 0f);
        }
        while (IsInsideBounds(spawnPosition));

        return spawnPosition;
    }

    private bool IsInsideBounds(Vector3 position)
    {
        return position.x >= minX && position.x <= maxX && position.y >= minY && position.y <= maxY;
    }
}
