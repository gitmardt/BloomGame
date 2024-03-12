using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;

    public SquareGrid grid;
    public GameObject enemyStorage;
    public GameObject pickupStorage;
    public Wave[] waves;
    public int testIndex = 0;
    public bool onStart = false;

    public int extraPickupRandomization = 2;

    public List<GameObject> enemies;
    public List<GameObject> pickups;

    public void Awake() => instance = this;

    public void Start() { if (onStart) StartWave(); }

    public void StartWave(int index) => StartCoroutine(WaveRoutine(waves[index]));

    [Button]
    public void StartWave() => StartCoroutine(WaveRoutine(waves[Mathf.Min(testIndex,waves.Length-1)]));

    IEnumerator WaveRoutine(Wave wave)
    {
        SpawnPickups(wave.pickups);

        for (int i = 0; i < wave.spawnMoments.Length; i++)
        {
            SpawnEnemies(wave.spawnMoments[i].enemies);
            yield return new WaitForSeconds(wave.spawnMoments[i].waitForSeconds);
        }
    }

    private void SpawnPickups(Wave.SpawnPickup[] pickups)
    {
        if (LevelGeneration.instance.pickupPoints.Count == 0) return;

        for (int i = 0; i < pickups.Length; i++)
        {
            int spawnLength = LevelGeneration.instance.pickupPoints.Count / pickups[i].amount;

            for (int u = 0; u < pickups[i].amount; u++)
            {
                int spawnIndex = u * spawnLength;

                if (spawnIndex + extraPickupRandomization < LevelGeneration.instance.pickupPoints.Count && spawnIndex - extraPickupRandomization > 0)
                {
                    spawnIndex += UnityEngine.Random.Range(-extraPickupRandomization, extraPickupRandomization);
                }

                if (spawnIndex >= LevelGeneration.instance.pickupPoints.Count || spawnIndex < 0) Debug.Log("SpawnIndex is not correct, it's " + spawnIndex + " and count is " + LevelGeneration.instance.pickupPoints.Count);

                Vector3 spawnPoint = LevelGeneration.instance.pickupPoints[spawnIndex];
                GameObject pickup = Instantiate(pickups[i].pickupType, spawnPoint + new Vector3(0, 3, 0), Quaternion.identity, pickupStorage.transform);
                pickup.name = pickups[i].pickupType.name + u;
                this.pickups.Add(pickup);
            }
        }
    }

    private void SpawnEnemies(Wave.SpawnEnemy[] enemies)
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            for (int u = 0; u < enemies[i].amount; u++)
            {
                Vector3 randomSpawnPoint = grid.borderPoints[UnityEngine.Random.Range(0, grid.borderPoints.Count)];
                GameObject Enemy = Instantiate(enemies[i].enemyType, randomSpawnPoint + new Vector3(0,5,0), Quaternion.identity, enemyStorage.transform);
                Enemy.name = enemies[i].enemyType.name + u;
                this.enemies.Add(Enemy);
            }
        }
    }
}
