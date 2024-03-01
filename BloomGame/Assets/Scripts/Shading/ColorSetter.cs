using UnityEngine;
using VInspector;

[ExecuteAlways, ExecuteInEditMode]
public class ColorSetter : MonoBehaviour
{
    [SerializeField] private Color Color;

    [SerializeField] private string colorProperty;

    [SerializeField] private Shader[] shader;

    public void ChangeColor()
    {
        //Change color for all materials in scene with the same shader
        foreach (Material material in Resources.FindObjectsOfTypeAll<Material>())
        {
            for (int i = 0; i < shader.Length; i++)
            {
                if (material.shader == shader[i])
                {
                    //Debug.Log("Changing color for " + material.name + " with shader " + shader[i].name + " to " + Color.ToString());
                    material.SetColor(colorProperty, Color);
                }
            }
        }
    }
}
