using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderSetter : MonoBehaviour
{
    [SerializeField] private PropertyType propertyType;
    

    [SerializeField] private string property;
    [SerializeField] private float Float;
    [SerializeField] private Vector3 Vector;
    [SerializeField] private Texture textureProperty;

    [SerializeField] private Shader[] shader;

    [SerializeField] private bool OnlyObjectsFromList;
    [SerializeField] private GameObject[] objects;

    public enum PropertyType
    {
        Float,
        Vector,
        Texture,
    }

    void ChangeProperty(Material material)
    {
        switch (propertyType)
        {
            case PropertyType.Float:
                material.SetFloat(property, Float);
                break;
            case PropertyType.Vector:
                material.SetVector(property, Vector);
                break;
            case PropertyType.Texture:
                material.SetTexture(property, textureProperty);
                break;
        }
    }

    public void ChangeColor()
    {
        if (OnlyObjectsFromList)
        {
            for (int i = 0; i < objects.Length; i++)
            {
#if UNITY_EDITOR
                Material material = objects[i].GetComponent<Renderer>().sharedMaterial;
#else
                Material material = objects[i].GetComponent<Renderer>().material;
#endif
                ChangeProperty(material);
            }
        }
        else
        {
            //Change color for all materials in scene with the same shader
            foreach (Material material in Resources.FindObjectsOfTypeAll<Material>())
            {
                for (int i = 0; i < shader.Length; i++)
                {
                    if (material.shader == shader[i])
                    {
                        ChangeProperty(material);
                    }
                }
            }
        }
    }
}
