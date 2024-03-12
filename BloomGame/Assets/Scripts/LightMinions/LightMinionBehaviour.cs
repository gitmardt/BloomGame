using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightMinionBehaviour : MonoBehaviour
{
    public float rotationSpeed = 1f;
    public Transform objectToRotate;
    private void Update()
    {
        if(WaveManager.instance.enemies.Count > 0)
        {
            GameObject nearestEnemy = NearestEnemy();
            if(nearestEnemy != null)
            {
                Vector3 direction = nearestEnemy.transform.position - objectToRotate.position;
                direction.y = 0;
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                objectToRotate.rotation = Quaternion.Lerp(objectToRotate.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }

    public GameObject NearestEnemy()
    {
        GameObject nearestEnemy = null;
        float nearestDistance = Mathf.Infinity;
        foreach(var enemy in WaveManager.instance.enemies)
        {
            float distance = Vector3.Distance(objectToRotate.position, enemy.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }
}
