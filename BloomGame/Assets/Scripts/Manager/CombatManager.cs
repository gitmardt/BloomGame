using Cinemachine;
using SmoothShakePro;
using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatManager : MonoBehaviour
{
    public static CombatManager instance;

    public LevelGeneration generator;
    public NavMeshSurface navMeshSurface;
    public GameObject menuEffectLayer;
    public GameObject mainMenuUI;
    public GameObject buttons, slider;
    public GameObject combatUI;
    public SmoothShakeCinemachine mainMenuPlayShake;
    public CinemachineVirtualCameraBase menuCam, combatCam;
    public float waveStartDelay = 1f;
    public bool navMeshIsGenerated = false;
    public bool gameover = false;

    private void Awake()
    {
        instance = this;
        navMeshIsGenerated = false;
        menuEffectLayer.SetActive(false);
        GameManager.OnGameStateChanged += OnGameStateChanged;
    }

    private void Update()
    {
        if(Player.instance.health == 0 && !gameover)
        {
            GameOver();
            gameover = true;
        }
    }

    private void GameOver()
    {
        FeedbackManager.instance.gameover.Play();
    }

    IEnumerator GameOverRoutine()
    {
        WaveManager.instance.waveTransition.StartShake();

        WaveManager.instance.gameOverText.SetActive(true);

        yield return new WaitForSeconds(WaveManager.instance.transitionTextDuration);

        TextReferences typewriter = WaveManager.instance.gameOverText.GetComponent<TextReferences>();
        for (int u = 0; u < typewriter.typewriters.Length; u++)
        {
            typewriter.typewriters[u].StartDisappearingText();
        }

        yield return new WaitForSeconds(3);

        WaveManager.instance.gameOverText.SetActive(false);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
        combatUI.SetActive(false);
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
        combatUI.SetActive(true);
        mainMenuUI.SetActive(false);
        navMeshSurface.BuildNavMesh();
        navMeshIsGenerated = true;
        StartCoroutine(WaveDelay());
    }

    IEnumerator WaveDelay()
    {
        yield return new WaitForSeconds(waveStartDelay);
        WaveManager.instance.StartWave();
    }
}
