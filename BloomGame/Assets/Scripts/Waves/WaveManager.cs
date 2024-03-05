using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

[System.Serializable]
public class SpawnEnemy
{
    public GameObject enemyType;
    public int amount = 1;
}

[System.Serializable]
public class SpawnMoment
{
    public SpawnEnemy[] enemies;
    public float waitForSeconds = 5;
}

public class WaveManager : MonoBehaviour
{
    public SquareGrid grid;
    public GameObject enemyStorage;
    public SpawnMoment[] waves;

    [Button]
    public void StartWave()
    {
        StartCoroutine(WaveRoutine());
    }

    IEnumerator WaveRoutine()
    {
        for (int i = 0; i < waves.Length; i++)
        {
            SpawnEnemies(waves[i].enemies);
            yield return new WaitForSeconds(waves[i].waitForSeconds);
        }
    }

    private void SpawnEnemies(SpawnEnemy[] enemies)
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            for (int u = 0; u < enemies[i].amount; u++)
            {
                Vector3 randomSpawnPoint = grid.borderPoints[Random.Range(0, grid.borderPoints.Count)];
                GameObject Enemy = Instantiate(enemies[i].enemyType, randomSpawnPoint, Quaternion.identity, enemyStorage.transform);
                Enemy.name = enemies[i].enemyType.name + u;
            }
        }
    }
}
