using Febucci.UI;
using SmoothShakePro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;

    public SmoothShake waveTransition;
    public float transitionTextDuration;
    public GameObject gameOverText;
    public GameObject[] waveTexts;
    public GameObject goalPrefab;
    public Vector2 minMaxDistanceFromPlayer = new Vector2(100, 400);
    public SquareGrid grid;
    public GameObject enemyStorage;
    public GameObject pickupStorage;
    public Wave[] waves;
    public Vector3 playerStartPosition = Vector3.zero;

    public Coroutine activeWave;

    public int currentWaveIndex = 0;

    public int extraPickupRandomization = 2;

    public List<GameObject> enemies;
    public List<GameObject> pickups;
    public GameObject EndGoal;

    public void Awake() => instance = this;

    public void StartWave()
    {
        if(activeWave == null)
        {
            StartCoroutine(WaveTransition());
        }
        else
        {
            Debug.LogWarning("Wave is already active, did you mean next wave?");
        }
    }

    public void NextWave()
    {
        if(activeWave != null) currentWaveIndex++;
        StopActiveWave();
        StartCoroutine(WaveTransition());
    }

    private IEnumerator WaveTransition()
    {
        if (currentWaveIndex + 1 > waves.Length)
        {
            Debug.LogWarning("No next wave available");
            yield break;
        }

        yield return WaveScreen();
        activeWave = StartCoroutine(WaveRoutine(waves[currentWaveIndex]));
    }

    private IEnumerator WaveScreen()
    {
        for (int i = 0; i < waveTexts.Length; i++)
        {
            if (i == currentWaveIndex)
            {
                waveTexts[i].SetActive(true);
            }
            else
            {
                waveTexts[i].SetActive(false);
            }
        }

        waveTransition.StartShake();

        Player.instance.transform.position = playerStartPosition;
        Player.instance.health = Player.instance.maxHealth;
        Player.instance.ammo = Player.instance.maxAmmo;
        Player.instance.lightAmmo = Player.instance.maxLightAmmo;
        Player.instance.maxLightAmmo++;

        yield return new WaitForSeconds(transitionTextDuration);

        waveTransition.StopShake();

        TextReferences typewriter = waveTexts[currentWaveIndex].GetComponent<TextReferences>();
        for (int u = 0; u < typewriter.typewriters.Length; u++)
        {
            typewriter.typewriters[u].StartDisappearingText();
        }

        yield return new WaitForSeconds(3);

        waveTexts[currentWaveIndex].SetActive(false);
    }

    public void StopActiveWave()
    {
        if (activeWave == null) return;

        DespawnGoal();
        for (int i = 0; i < enemies.Count; i++)
        {
            Destroy(enemies[i]);
        }
        enemies.Clear();
        StopCoroutine(activeWave);
        activeWave = null;
    }

    IEnumerator WaveRoutine(Wave wave)
    {
        Player.instance.minimumGoalViewDistance = wave.minDistanceForHint;
        SpawnPickups(wave.pickups);

        switch (currentWaveIndex)
        {
            case 0:
                StartCoroutine(TextBarManager.instance.PlayDialogue("intro")); 
                break;
            case 1:
                StartCoroutine(TextBarManager.instance.PlayDialogue("wave2"));
                break;
            case 2:
                StartCoroutine(TextBarManager.instance.PlayDialogue("wave3"));
                break;
            case 3:
                StartCoroutine(TextBarManager.instance.PlayDialogue("wave4"));
                break;
            case 4:
                StartCoroutine(TextBarManager.instance.PlayDialogue("wave5"));
                break;

        }

        yield return new WaitForSeconds(8);

        for (int i = 0; i < wave.spawnMoments.Length; i++)
        {
            SpawnEnemies(wave.spawnMoments[i].enemies);
            if (wave.spawnMoments[i].spawnGoal) SpawnGoal();
            yield return new WaitForSeconds(wave.spawnMoments[i].waitForSeconds);
        }

        if (EndGoal == null)
        {
            Debug.LogWarning("Uhm this shouldnt happen");
            yield break;
        }

        Goal goal = EndGoal.GetComponent<Goal>();

        if (!goal.success)
        {
            Player.instance.minimumGoalViewDistance = 5000;
            SpawnPickups(wave.pickups);
        }

        while (!goal.success)
        {
            SpawnEnemies(wave.spawnMoments[^1].enemies);
            yield return new WaitForSeconds(wave.spawnMoments[^1].waitForSeconds);
        }
    }

    private void SpawnGoal()
    {
        DespawnGoal();

        Vector3 randomPosition = RandomPosition(LevelGeneration.instance.pickupPoints);

        EndGoal = Instantiate(goalPrefab, randomPosition, Quaternion.identity);
        EndGoal.name = "EndGoalObject";
    }

    private void DespawnGoal()
    {
        if(EndGoal != null) Destroy(EndGoal);
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
