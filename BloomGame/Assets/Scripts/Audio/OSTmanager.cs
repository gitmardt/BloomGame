using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSTmanager : MonoBehaviour
{
    public static OSTmanager instance;

    public AudioSource mainTheme, battleAmbience, battleTheme;

    private AudioSource activeAudioSource;

    public float musicVolume = 0.5f;
    public float fadeDuration = 2;

    private GameState activeState = GameState.MainMenu;

    public float battleThemeRange = 50;

    private bool hasSwitched = false;

    private void Awake()
    {
        instance = this;
        GameManager.OnGameStateChanged += OnGameStateChanged;
    }

    private void Update()
    {
        if(activeState == GameState.Combat)
        {
            if(WaveManager.instance.enemies.Count > 0)
            {
                if (Vector3.Distance(GetNearestEnemy.instance.NearestEnemy(Player.instance.transform).transform.position, Player.instance.transform.position) < battleThemeRange && !hasSwitched)
                {
                    StartCoroutine(SwitchMusic(battleTheme));
                    hasSwitched = true;
                }
                else if (Vector3.Distance(GetNearestEnemy.instance.NearestEnemy(Player.instance.transform).transform.position, Player.instance.transform.position) >= battleThemeRange && hasSwitched)
                {
                    StartCoroutine(SwitchMusic(battleAmbience));
                    hasSwitched = false;
                }
            }
        }
    }

    private void OnGameStateChanged(GameState state)
    {
        activeState = state;

        if (state == GameState.MainMenu)
        {
            StartCoroutine(SwitchMusic(mainTheme));
        }

        if (state == GameState.Generating)
        {
            StartCoroutine(SwitchMusic(battleAmbience));
        }
    }

    public IEnumerator SwitchMusic(AudioSource audio)
    {
        if(activeAudioSource != null)
        {
            if (activeAudioSource == audio) yield break;

            if (activeAudioSource.isPlaying)
            {
                activeAudioSource.DOFade(0, fadeDuration);
                yield return new WaitForSeconds(fadeDuration);
            }
        }

        activeAudioSource = audio;
        activeAudioSource.Play();
        activeAudioSource.DOFade(musicVolume, fadeDuration);
    }

}
