using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Goal : MonoBehaviour
{
    public float currentCharge = 0;
    public float chargeTime = 50;
    public float activationDistance = 20;
    public Coroutine activeRoutine;

    public DecalProjector groundProjection;
    public MeshRenderer fireOrb;

    private Material groundProj, fireMat;

    public string fireOrbProperty, groundProjProperty;

    public Vector2 fireOrbRemapRange;
    public Vector2 groudnProjectionRemapRange;

    private void Start()
    {
        fireMat = fireOrb.material;
        groundProj = groundProjection.material;
        fireMat.SetFloat(fireOrbProperty, Utility.Remap(currentCharge, 0, chargeTime, fireOrbRemapRange.x, fireOrbRemapRange.y));
        groundProj.SetFloat(groundProjProperty, Utility.Remap(currentCharge, 0, chargeTime, groudnProjectionRemapRange.x, groudnProjectionRemapRange.y));
    }

    private void OnEnable()
    {
        fireMat = fireOrb.material;
        groundProj = groundProjection.material;
        fireMat.SetFloat(fireOrbProperty, Utility.Remap(currentCharge, 0, chargeTime, fireOrbRemapRange.x, fireOrbRemapRange.y));
        groundProj.SetFloat(groundProjProperty, Utility.Remap(currentCharge, 0, chargeTime, groudnProjectionRemapRange.x, groudnProjectionRemapRange.y));
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position,Player.instance.transform.position) < activationDistance && activeRoutine == null)
        {
            activeRoutine = StartCoroutine(Activate());
        }
    }

    public IEnumerator Activate()
    {
        while(currentCharge < chargeTime)
        {
            fireMat.SetFloat(fireOrbProperty,Utility.Remap(currentCharge,0,chargeTime,fireOrbRemapRange.x,fireOrbRemapRange.y));
            groundProj.SetFloat(groundProjProperty, Utility.Remap(currentCharge, 0, chargeTime, groudnProjectionRemapRange.x, groudnProjectionRemapRange.y));
            Player.instance.goalUISlider.fillAmount = Utility.Remap(currentCharge, 0, chargeTime, 0, 1);

            yield return null;
            currentCharge += Time.deltaTime;
        }
        WaveManager.instance.NextWave();
    }
}
