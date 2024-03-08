using Unity.AI.Navigation;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public LevelGeneration generator;
    public NavMeshSurface navMeshSurface;

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
        WaveManager.instance.StartWave(0);
    }
}
