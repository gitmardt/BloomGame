using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightMinionSpawner : MonoBehaviour
{
    public static LightMinionSpawner instance;

    public GameObject transparentLightMinionObj;

    private void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        transparentLightMinionObj.transform.position = Player.instance.mousePosition;
    }
}
