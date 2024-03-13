using SmoothShakePro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spiker : EnemyBase
{
    public Transform hitPosition;
    public SmoothShake attackShake;
    public override float AttackDuration() => attackShake.timeSettings.GetShakeDuration();
    public override void DealDamage()
    {
        Debug.Log("DealDamage?");
        if (Vector3.Distance(player.position, hitPosition.position) < damageRange)
        {
            FeedbackManager.instance.ShakePostProcessing(layerIndex);
            Player.instance.health--;
            FeedbackManager.instance.hitShake.StartShake(FeedbackManager.instance.hitShakePlayer);
            FeedbackManager.instance.playerhit.Play();
        }
    }
}
