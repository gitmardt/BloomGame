using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightMinionSpawner : MonoBehaviour
{
    public static LightMinionSpawner instance;

    public GameObject transparentLightMinionObj;
    public GameObject lightMinionSpawnObj;
    public GameObject lightMinionParentFolder;
    public DecalProjector decalProjector;
    public Vector3 decalProjectorScale;
    public float tweenDuration = 0.5f;

    private List<GameObject> lightMinions = new();

    public bool spawnMode = false;

    private Coroutine activeRoutine = null;

    private void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        SpawnMode();
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
        if (activeRoutine != null) StopCoroutine(activeRoutine);
        activeRoutine = null;
        transparentLightMinionObj.SetActive(true);
        DOVirtual.Float(0, 1, tweenDuration, x => { decalProjector.fadeFactor = x; });
    }

    public void StopSpawning()
    {
        activeRoutine = StartCoroutine(StopRoutine());
    }

    private IEnumerator StopRoutine()
    {
        DOVirtual.Float(1, 0, tweenDuration, x => { decalProjector.fadeFactor = x; });
        yield return new WaitForSeconds(tweenDuration);
        transparentLightMinionObj.SetActive(false);
    }
}
