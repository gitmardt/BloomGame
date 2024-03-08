using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ShaderSetter))]
public class ShaderSetterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ShaderSetter shaderSetter = (ShaderSetter)target;

        //Button
        if (GUILayout.Button("Change Color"))
        {
            shaderSetter.ChangeColor();
        }

        if (GUI.changed)
            shaderSetter.ChangeColor();
    }
}
