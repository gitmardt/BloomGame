using Cinemachine;
using SmoothShakePro;
using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public LevelGeneration generator;
    public NavMeshSurface navMeshSurface;
    public GameObject menuEffectLayer;
    public GameObject mainMenuUI;
    public GameObject buttons, slider;
    public SmoothShakeCinemachine mainMenuPlayShake;
    public CinemachineVirtualCameraBase menuCam, combatCam;
    public float waveStartDelay = 1f;

    private void Awake()
    {
        menuEffectLayer.SetActive(false);
        GameManager.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState state)
    {
        if (state == GameState.Generating)
        {
            GenerateLevel();
        }
    }

    public void GenerateLevel()
    {
        buttons.SetActive(false);
        slider.SetActive(true);
        menuEffectLayer.SetActive(true);
        mainMenuPlayShake.StartShake();
        StartCoroutine(StartDelay());
    }

    IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(0.2f);
        generator.ClearGrid();
        yield return generator.GenerateLevel();

        mainMenuPlayShake.StopShake();
        menuEffectLayer.SetActive(false);
        menuCam.Priority = 0;
        combatCam.Priority = 1;
        mainMenuUI.SetActive(false);
        navMeshSurface.BuildNavMesh();
        StartCoroutine(WaveDelay());
    }

    IEnumerator WaveDelay()
    {
        yield return new WaitForSeconds(waveStartDelay);
        WaveManager.instance.StartWave(0);
    }
}
