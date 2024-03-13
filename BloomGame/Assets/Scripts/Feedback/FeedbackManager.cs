using SmoothShakePro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackManager : MonoBehaviour
{
    public static FeedbackManager instance;

    public ShakeBase hitShake;
    public ShakeBasePreset hitShakeEnemy, hitShakePlayer, EnemyDieShake;

    private void Awake()
    {
        instance = this;
    }
}
