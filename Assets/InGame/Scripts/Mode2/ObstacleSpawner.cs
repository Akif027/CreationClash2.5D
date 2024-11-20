using UnityEngine;
using System.Collections.Generic;

public class ObstacleSpawner : MonoBehaviour
{
    public ObstacleDataCollection obstacleDataCollection;  // Reference to a single ScriptableObject holding all obstacles
    public GameObject bossPrefab;                          // Boss prefab
    public int maxObstacles = 10;                          // Max obstacles allowed in the level
    public int totalObstaclesForBoss = 20;                 // Total obstacles to spawn before the boss appears
    public float spawnInterval = 2f;                       // Interval between obstacle spawns
    public GameObject[] spawnPoints;                       // Array of GameObject spawn points
    public float obstacleLifetime = 5f;                    // Lifetime for obstacles
    public float bossLifetime = 10f;                       // Lifetime for the boss

    private Dictionary<ObstacleDataCollection.ObstacleType, int> obstacleWeights = new Dictionary<ObstacleDataCollection.ObstacleType, int>();
    private int obstacleCount = 0;                         // Counter for the number of obstacles spawned
    private bool bossSpawned = false;                      // Flag to ensure the boss spawns only once

    void Start()
    {
        InitializeObstacleWeights();
        InvokeRepeating(nameof(SpawnObstacle), 1f, spawnInterval);
    }

    // Initialize the weights for each obstacle based on the obstacleDataCollection
    void InitializeObstacleWeights()
    {
        foreach (var obstacleType in obstacleDataCollection.obstacles)
        {
            if (!obstacleWeights.ContainsKey(obstacleType))
            {
                obstacleWeights.Add(obstacleType, obstacleType.weight);
            }
        }
    }

    // Method to spawn an obstacle
    void SpawnObstacle()
    {
        // Stop spawning obstacles if the boss has been spawned or if max obstacles are on screen
        if (bossSpawned || obstacleCount >= maxObstacles) return;

        // Check if it's time to spawn the boss
        if (obstacleCount >= totalObstaclesForBoss && !bossSpawned)
        {
            SpawnBoss();
            bossSpawned = true;
            CancelInvoke(nameof(SpawnObstacle)); // Stop regular obstacle spawning after boss appears
            return;
        }

        // Get a randomly weighted obstacle
        var selectedObstacle = GetWeightedRandomObstacle();

        if (selectedObstacle != null)
        {
            // Randomly choose a spawn point GameObject from the array
            GameObject spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Vector2 spawnPosition = spawnPoint.transform.position;

            // Use the ObjectPool to get the obstacle object
            GameObject obstacle = ObjectPool.Instance.GetPooledObject("Obstacle1");
            if (obstacle != null)
            {
                obstacle.transform.position = spawnPosition;

                // Set up the obstacle mover with appropriate lifetime
                ObstacleMover mover = obstacle.GetComponent<ObstacleMover>();
                if (mover == null)
                {
                    mover = obstacle.AddComponent<ObstacleMover>();
                }
                mover.SetLifetime(obstacleLifetime);

                obstacleCount++;
            }
        }
    }

    // Method to get a random obstacle based on their weights
    ObstacleDataCollection.ObstacleType GetWeightedRandomObstacle()
    {
        int totalWeight = 0;

        // Calculate total weight
        foreach (var weight in obstacleWeights.Values)
        {
            totalWeight += weight;
        }

        // Generate a random weight point
        int randomWeightPoint = Random.Range(0, totalWeight);
        int currentWeight = 0;

        // Select an obstacle based on the random weight point
        foreach (var entry in obstacleWeights)
        {
            currentWeight += entry.Value;
            if (randomWeightPoint < currentWeight)
            {
                return entry.Key;
            }
        }

        return null;
    }

    // Method to spawn the boss
    public void SpawnBoss()
    {
        // Randomly choose a spawn point GameObject for the boss
        GameObject spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Vector2 spawnPosition = spawnPoint.transform.position;

        // Use ObjectPool to spawn the boss
        GameObject boss = ObjectPool.Instance.GetPooledObject("boss");
        if (boss != null)
        {
            boss.transform.position = spawnPosition;

            // Set up the boss mover with appropriate lifetime
            BossMover mover = boss.GetComponent<BossMover>();
            if (mover == null)
            {
                mover = boss.AddComponent<BossMover>();
            }
            mover.SetLifetime(bossLifetime);
        }
    }

    // Method to return the obstacle to the pool after its lifetime
    public void ReturnObstacle(GameObject obstacle)
    {
        ObjectPool.Instance.ReturnObjectToPool(obstacle, obstacle.name);
        obstacleCount--;
    }
}
