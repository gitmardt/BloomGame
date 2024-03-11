using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightMinionSpawner : MonoBehaviour
{
    public static LightMinionSpawner instance;

    public GameObject transparentLightMinionObj;
    public GameObject lightMinionSpawnObj;
    public GameObject lightMinionParentFolder;

    private List<GameObject> lightMinions = new();

    public bool spawnMode = false;

    private void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnMode) SpawnMode();
    }

    void SpawnMode()
    {
        transparentLightMinionObj.transform.position = Player.instance.mousePosition;
    }

    public void SpawnLight()
    {
        GameObject lightObject = Instantiate(lightMinionSpawnObj, transparentLightMinionObj.transform.position, transparentLightMinionObj.transform.rotation, lightMinionParentFolder.transform);
        lightMinions.Add(lightObject);
        lightObject.name = "LightMinion" + lightMinions.Count;
    }

    public void StartSpawning()
    {
        transparentLightMinionObj.SetActive(true);
    }

    public void StopSpawning()
    {
        transparentLightMinionObj.SetActive(false);
    }
}
