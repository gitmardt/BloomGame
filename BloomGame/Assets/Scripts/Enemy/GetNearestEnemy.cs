using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetNearestEnemy : MonoBehaviour
{
    public static GetNearestEnemy instance;

    private void Awake()
    {
        instance = this;
    }

    public GameObject NearestEnemy(Transform referencePosition)
    {
        if (WaveManager.instance.enemies.Count > 0)
        {
            GameObject nearestEnemy = null;
            float nearestDistance = Mathf.Infinity;
            foreach (var enemy in WaveManager.instance.enemies)
            {
                float distance = Vector3.Distance(referencePosition.position, enemy.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestEnemy = enemy;
                }
            }

            return nearestEnemy;
        }
        else return null;
    }
}
