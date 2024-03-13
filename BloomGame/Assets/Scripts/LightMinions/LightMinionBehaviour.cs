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
            GameObject nearestEnemy = GetNearestEnemy.instance.NearestEnemy(objectToRotate.transform);
            if(nearestEnemy != null)
            {
                Vector3 direction = nearestEnemy.transform.position - objectToRotate.position;
                direction.y = 0;
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                objectToRotate.rotation = Quaternion.Lerp(objectToRotate.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }
}
