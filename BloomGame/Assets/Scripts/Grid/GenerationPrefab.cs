using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable, CreateAssetMenu(fileName = "SpriteAnimation", menuName = "Custom/GenerationPrefab")]
public class GenerationPrefab : ScriptableObject
{
    public GameObject prefab;
    public Vector3[] randomRotations;
    public bool allRotations;
    public bool randomScale = true;
    public float scaleModifierMin = -0.2f;
    public float scaleModifierMax = 0.2f;
    [Range(0,1)] public float percentChange = 0.1f;
}
