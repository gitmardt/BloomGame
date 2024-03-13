using SmoothShakePro;
using UnityEngine;

public class FeedbackManager : MonoBehaviour
{
    public static FeedbackManager instance;

    public ShakeBase hitShake;
    public ShakeBasePreset hitShakeEnemy, hitShakePlayer, EnemyDieShake;
    public AudioSource enemyDeath1, enemyDeath2, ammoPickup, noAmmo, placeLight, removeLight, gameover, playerhit;

    public SmoothShakePostProcessing sspp;
    public SmoothShakePostProcessingPreset warpingShake, twitchShake, bloomShake, quantizationShake, bloomShot;

    private void Awake()
    {
        instance = this;
    }

    public void ShakePostProcessing(int layerindex)
    {
        switch (layerindex)
        {
            case 6:
                sspp.StartShake(quantizationShake);
                break;
            case 7:
                sspp.StartShake(bloomShake);
                break;
            case 8:
                sspp.StartShake(warpingShake);
                break;
            case 11:
                sspp.StartShake(twitchShake);
                break;
        }
    }
}
