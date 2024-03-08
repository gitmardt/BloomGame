using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable, CreateAssetMenu(fileName = "SpriteAnimation", menuName = "Custom/GenerationPrefab")]
public class GenerationPrefab : ScriptableObject
{
    public GameObject prefab;
    public Vector3[] randomRotations;
    public bool allRotations;
}
