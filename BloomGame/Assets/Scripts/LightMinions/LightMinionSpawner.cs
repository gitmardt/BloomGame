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
    public float removalDistance = 5;
    public float smoothAimDelay = 0.5f;

    private List<GameObject> lightMinions = new();
    private int removalIndex = 0;

    public ActiveMode activeMode = ActiveMode.off;

    private Material groundCircle;

    public enum ActiveMode
    {
        spawning,
        removal,
        off
    }

    private Coroutine activeRoutine = null;

    private void Awake()
    {
        instance = this;
        groundCircle = decalProjector.material;
    }

    // Update is called once per frame
    void Update()
    {
        transparentLightMinionObj.transform.position = 
            Vector3.Lerp(transparentLightMinionObj.transform.position, Player.instance.mousePosition, Time.deltaTime * smoothAimDelay);

        if (activeMode == ActiveMode.spawning || activeMode == ActiveMode.removal) SpawnMode();
    }

    void SpawnMode()
    {
        if (lightMinions.Count == 0)
        {
            activeMode = ActiveMode.spawning;
            groundCircle.SetFloat("_removal", 1);
            return;
        }

        bool withinRemoval = false;

        for (int i = 0; i < lightMinions.Count; i++)
        {
            if (Vector3.Distance(lightMinions[i].transform.position, transparentLightMinionObj.transform.position) <= removalDistance)
            {
                activeMode = ActiveMode.removal;
                groundCircle.SetFloat("_removal", 0);
                removalIndex = i;
                withinRemoval = true;
                break;
            }
        }

        if (!withinRemoval)
        {
            activeMode = ActiveMode.spawning;
            groundCircle.SetFloat("_removal", 1);
        }
    }

    public void SpawnLight()
    {
        switch (activeMode)
        {
            case ActiveMode.spawning:
                GameObject lightObject = Instantiate(lightMinionSpawnObj, transparentLightMinionObj.transform.position, transparentLightMinionObj.transform.rotation, lightMinionParentFolder.transform);
                lightMinions.Add(lightObject);
                lightObject.name = "LightMinion" + lightMinions.Count;
                break;
            case ActiveMode.removal:
                Destroy(lightMinions[removalIndex]);
                lightMinions.RemoveAt(removalIndex);
                break;
        }
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
