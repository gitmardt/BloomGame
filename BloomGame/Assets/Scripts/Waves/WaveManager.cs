using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class WaveManager : MonoBehaviour
{
    public SquareGrid grid;
    public GameObject enemyStorage;
    public Wave[] waves;
    int testIndex = 1;

    public void StartWave(int index) => StartCoroutine(WaveRoutine(waves[index]));

    [Button]
    public void StartWave() => StartCoroutine(WaveRoutine(waves[testIndex]));

    IEnumerator WaveRoutine(Wave wave)
    {
        for (int i = 0; i < wave.spawnMoments.Length; i++)
        {
            SpawnEnemies(wave.spawnMoments[i].enemies);
            yield return new WaitForSeconds(wave.spawnMoments[i].waitForSeconds);
        }
    }

    private void SpawnEnemies(Wave.SpawnEnemy[] enemies)
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            for (int u = 0; u < enemies[i].amount; u++)
            {
                Vector3 randomSpawnPoint = grid.borderPoints[UnityEngine.Random.Range(0, grid.borderPoints.Count)];
                GameObject Enemy = Instantiate(enemies[i].enemyType, randomSpawnPoint, Quaternion.identity, enemyStorage.transform);
                Enemy.name = enemies[i].enemyType.name + u;
            }
        }
    }
}
