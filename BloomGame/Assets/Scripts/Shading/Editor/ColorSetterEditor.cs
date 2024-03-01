using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ColorSetter))]
public class ColorSetterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ColorSetter colorSetter = (ColorSetter)target;

        //Button
        if (GUILayout.Button("Change Color"))
        {
            colorSetter.ChangeColor();
        }

        if (GUI.changed)
            colorSetter.ChangeColor();
    }
}
