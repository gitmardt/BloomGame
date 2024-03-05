using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable, CreateAssetMenu(fileName = "SpriteAnimation", menuName = "Custom/SpriteAnimation")]
public class SpriteAnimation : ScriptableObject
{
    public Sprite[] sprites;
    public float animationSpeed = 16f;
}
