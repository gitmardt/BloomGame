using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public LevelGeneration generator;
    public NavMeshSurface navMeshSurface;
    public float waveStartDelay = 1f;

    private void Awake()
    {
        GameManager.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState state)
    {
        if (state == GameState.Combat)
        {
            GenerateLevel();
        }
    }

    public void GenerateLevel()
    {
        generator.ClearGrid();
        generator.GenerateLevel();
        navMeshSurface.BuildNavMesh();
        StartCoroutine(WaveDelay());
    }

    IEnumerator WaveDelay()
    {
        yield return new WaitForSeconds(waveStartDelay);
        WaveManager.instance.StartWave(0);
    }
}
