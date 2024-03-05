using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VInspector;

[RequireComponent(typeof(SquareGrid))]
public class LevelGeneration : MonoBehaviour
{
    SquareGrid squareGrid;

    public GameObject environmentFolder;
    public List<GenerationPrefab> environmentPrefabs = new();

    private List<GameObject> scatteredObjects = new();

    private SquareGrid GetGrid()
    {
        if(squareGrid == null) squareGrid = GetComponent<SquareGrid>();
        squareGrid.CreateGrid();
        return squareGrid;
    }

    [Button]
    public void GenerateLevel()
    {
        SquareGrid grid = GetGrid();

        ScatterAssets(grid);
    }

    private void ScatterAssets(SquareGrid grid)
    {
        if (environmentPrefabs == null || environmentPrefabs.Count == 0) { 
            Debug.LogWarning("Can't generate because Environment Prefabs list is empty"); 
            return; }

        for (int i = 0; i < scatteredObjects.Count; i++)
        {
#if UNITY_EDITOR
            DestroyImmediate(scatteredObjects[i]);
#else
            Destroy(scatteredObjects[i]);
#endif
        }

        scatteredObjects.Clear();

        for (int i = 0; i < grid.randomlyPickedPoints.Count; i++)
        {
            int random = Random.Range(0, environmentPrefabs.Count);
            GameObject prefabToPlace = environmentPrefabs[random].prefab;
            GameObject instancedPrefab = Instantiate(prefabToPlace, grid.randomlyPickedPoints[i], 
                Quaternion.Euler(environmentPrefabs[random].randomRotations[Random.Range(0, environmentPrefabs[random].randomRotations.Length)]),
                environmentFolder.transform);
            instancedPrefab.name = "RandomlyPlacedObject" + i;
            scatteredObjects.Add(instancedPrefab);
        }

    }
}
