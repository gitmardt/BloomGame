using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "Wave", menuName = "Custom/Wave")]
public class Wave : ScriptableObject
{
    public SpawnMoment[] spawnMoments;
    public SpawnPickup[] pickups;

    [System.Serializable]
    public class SpawnEnemy
    {
        public GameObject enemyType;
        public int amount = 1;
    }

    [System.Serializable]
    public class SpawnPickup
    {
        public GameObject pickupType;
        public int amount = 1;
    }

    [System.Serializable]
    public class SpawnMoment
    {
        public SpawnEnemy[] enemies;
        public float waitForSeconds = 5;
    }
}
