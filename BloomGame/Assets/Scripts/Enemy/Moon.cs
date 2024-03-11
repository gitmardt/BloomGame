using SmoothShakePro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moon : EnemyBase
{
    public Transform hitPosition;
    public float attackRange = 5;
    public AnimationClip attackClip;
    public override float AttackDuration() => attackClip.length;
    public override void DealDamage()
    {
        if (Vector3.Distance(player.position, hitPosition.position) < attackRange)
        {
            Player.instance.health--;
            Debug.Log("Player health is " + Player.instance.health);
        }
    }
}
