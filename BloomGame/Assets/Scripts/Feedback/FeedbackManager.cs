using SmoothShakePro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackManager : MonoBehaviour
{
    public static FeedbackManager instance;

    public ShakeBase hitShake;
    public ShakeBasePreset hitShakeEnemy, hitShakePlayer, EnemyDieShake;
    public AudioSource enemyDeath1, enemyDeath2;

    private void Awake()
    {
        instance = this;
    }
}
