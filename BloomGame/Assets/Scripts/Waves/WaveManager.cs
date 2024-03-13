using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;

    public GameObject goalPrefab;
    public Vector2 minMaxDistanceFromPlayer = new Vector2(100, 400);
    public SquareGrid grid;
    public GameObject enemyStorage;
    public GameObject pickupStorage;
    public Wave[] waves;
    public int testIndex = 0;
    public bool onStart = false;

    public Coroutine activeWave;

    private int currentWaveIndex = 0;

    public int extraPickupRandomization = 2;

    public List<GameObject> enemies;
    public List<GameObject> pickups;
    public GameObject EndGoal;

    public void Awake() => instance = this;

    public void Start() { if (onStart) StartWave(); }

    public void StartWave(int index)
    {
        if(activeWave == null)
        {
            activeWave = StartCoroutine(WaveRoutine(waves[index], index));
        }
        else
        {
            Debug.LogWarning("Wave is already active, did you mean next wave?");
        }
    }

    [Button]
    public void StartWave()
    {
        if (activeWave == null)
        {
            activeWave = StartCoroutine(WaveRoutine(waves[Mathf.Min(testIndex, waves.Length - 1)], testIndex));
        }
        else
        {
            Debug.LogWarning("Wave is already active, did you mean next wave?");
        }
    }

    public void NextWave()
    {
        StopActiveWave();

        if (currentWaveIndex + 1 >= waves.Length)
        {
            Debug.LogWarning("No next wave available");
            return;
        }

        currentWaveIndex++;
        activeWave = StartCoroutine(WaveRoutine(waves[currentWaveIndex], currentWaveIndex));
    }

    public void StopActiveWave()
    {
        StopCoroutine(activeWave);
        activeWave = null;
    }

    IEnumerator WaveRoutine(Wave wave, int index)
    {
        currentWaveIndex = index;
        SpawnPickups(wave.pickups);

        for (int i = 0; i < wave.spawnMoments.Length; i++)
        {
            SpawnEnemies(wave.spawnMoments[i].enemies);
            if (wave.spawnMoments[i].spawnGoal) SpawnGoal();
            yield return new WaitForSeconds(wave.spawnMoments[i].waitForSeconds);
        }
    }

    private void SpawnGoal()
    {
        if (EndGoal != null) DespawnGoal();

        Vector3 randomPosition = RandomPosition(LevelGeneration.instance.pickupPoints);

        EndGoal = Instantiate(goalPrefab, randomPosition, Quaternion.identity);
        EndGoal.name = "EndGoalObject";
    }

    private void DespawnGoal()
    {
        Destroy(EndGoal);
    }

    private Vector3 RandomPosition(List<Vector3> pointList)
    {
        int random = Random.Range(0, pointList.Count);

        while (Vector3.Distance(pointList[random], Player.instance.transform.position) < minMaxDistanceFromPlayer.x && 
            Vector3.Distance(pointList[random], Player.instance.transform.position) > minMaxDistanceFromPlayer.y)
        {
            random = Random.Range(0, pointList.Count);
        }

        return pointList[random];
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
                    spawnIndex += Random.Range(-extraPickupRandomization, extraPickupRandomization);
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
