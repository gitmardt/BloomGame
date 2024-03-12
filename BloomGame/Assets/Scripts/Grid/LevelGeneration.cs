using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VInspector;

[RequireComponent(typeof(SquareGrid))]
public class LevelGeneration : MonoBehaviour
{
    public Slider loadingBar;
    private float steps, currentStep;

    private bool finishedGenerating = false;

    public static LevelGeneration instance;

    SquareGrid squareGrid;

    public GameObject environmentFolder;
    public List<GenerationPrefab> environmentPrefabs = new();
    public List<GenerationPrefab> vfxPrefabs = new();

    public List<GameObject> scatteredObjects = new();

    public List<Vector3> pickupPoints = new();

    private void Awake() => instance = this;

    private SquareGrid GetGrid()
    {
        if(squareGrid == null) squareGrid = GetComponent<SquareGrid>();
        squareGrid.CreateGrid();
        return squareGrid;
    }

    [Button]
    public IEnumerator GenerateLevel()
    {
        if (environmentPrefabs == null || environmentPrefabs.Count == 0)
        {
            Debug.LogWarning("Can't generate because Environment Prefabs list is empty");
            yield break;
        }

        SquareGrid grid = GetGrid();

        yield return ScatterAssets(grid);
    }

    [Button]
    public void ClearGrid()
    {
        for (int i = 0; i < scatteredObjects.Count; i++)
        {
#if UNITY_EDITOR
            DestroyImmediate(scatteredObjects[i]);
#else
            Destroy(scatteredObjects[i]);
#endif
        }

        scatteredObjects.Clear();
    }

    private IEnumerator ScatterAssets(SquareGrid grid)
    {
        loadingBar.gameObject.SetActive(true);
        steps = 3;
        currentStep = 0;

        ClearGrid();
        StartCoroutine(ScatterRoutine(grid));
        currentStep++;
        loadingBar.value = currentStep / steps;
        yield return null;
        StartCoroutine(SpawnSmoke(grid));
        currentStep++;
        loadingBar.value = currentStep / steps;
        yield return null;
        StartCoroutine(SetPickupPoints(grid));
        currentStep++;
        loadingBar.value = currentStep / steps;

        finishedGenerating = true;
        loadingBar.gameObject.SetActive(false);
        GameManager.instance.UpdateGameState(GameState.Combat);
    }

    private IEnumerator ScatterRoutine(SquareGrid grid)
    {
        for (int i = 0; i < grid.randomlyPickedPoints.Count; i++)
        {
            int random = Random.Range(0, environmentPrefabs.Count);

            for (int u = 0; u < environmentPrefabs.Count; u++)
            {
                if (environmentPrefabs[u].percentChange > Random.value)
                {
                    random = u;
                }
            }

            GameObject prefabToPlace = environmentPrefabs[random].prefab;

            Quaternion randomRotation;
            if (!environmentPrefabs[random].allRotations) randomRotation = Quaternion.Euler(environmentPrefabs[random].randomRotations[Random.Range(0, environmentPrefabs[random].randomRotations.Length)]);
            else randomRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);


            GameObject instancedPrefab = Instantiate(prefabToPlace, grid.randomlyPickedPoints[i], randomRotation, environmentFolder.transform);

            if (environmentPrefabs[random].randomScale) instancedPrefab.transform.localScale *= Random.Range(1 + environmentPrefabs[random].scaleModifierMin, 1 + environmentPrefabs[random].scaleModifierMax);

            instancedPrefab.name = "RandomlyPlacedObject" + environmentPrefabs[random].prefab.name + "_" + i;
            scatteredObjects.Add(instancedPrefab);

            yield return null; 
        }
    }

    private IEnumerator SetPickupPoints(SquareGrid grid)
    {
        for (int i = 0; i < grid.points.Count; i++)
        {
            if (!grid.randomlyPickedPoints.Contains(grid.points[i]))
            {
                for (int u = 0; u < scatteredObjects.Count; u++)
                {
                    if (scatteredObjects[u].GetComponent<Collider>() != null)
                    {
                        if (!scatteredObjects[u].GetComponent<Collider>().bounds.Contains(grid.points[i]))
                        {
                            if (!pickupPoints.Contains(grid.points[i]))
                            {
                                pickupPoints.Add(grid.points[i]);
                                yield return null; 
                            }
                        }
                    }
                }
            }
        }
    }

    private IEnumerator SpawnSmoke(SquareGrid grid)
    {
        for (int i = 0; i < grid.points.Count; i++)
        {
            if (!grid.randomlyPickedPoints.Contains(grid.points[i]))
            {
                int random = Random.Range(0, vfxPrefabs.Count);

                bool spawn = true;

                for (int u = 0; u < vfxPrefabs.Count; u++)
                {
                    if (vfxPrefabs[u].percentChange > Random.value) random = u;
                    else spawn = false;
                }

                if (spawn)
                {
                    GameObject prefabToPlace = vfxPrefabs[random].prefab;
                    GameObject instancedPrefab = Instantiate(prefabToPlace, grid.points[i] + prefabToPlace.transform.position, Quaternion.identity, environmentFolder.transform);

                    if (vfxPrefabs[random].randomScale) instancedPrefab.transform.localScale *= Random.Range(1 + vfxPrefabs[random].scaleModifierMin, 1 + vfxPrefabs[random].scaleModifierMax);

                    instancedPrefab.name = "VFX_" + vfxPrefabs[random].prefab.name + "_" + i;
                    scatteredObjects.Add(instancedPrefab);
                    yield return null; 
                }
            }
        }
    }
}
