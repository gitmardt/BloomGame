using SmoothShakePro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moon : EnemyBase
{
    public Transform hitPosition;
    public AnimationClip attackClip;
    public override float AttackDuration() => attackClip.length;
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
