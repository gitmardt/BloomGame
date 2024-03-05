using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(SquareGrid))]
public class SquareGridEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SquareGrid grid = (SquareGrid)target;

        base.OnInspectorGUI();

        if(GUILayout.Button("Create Grid"))
        {
            grid.CreateGrid();
        }

        if (grid.dynamicEditing)
        {
            if (GUI.changed)
            {
                grid.CreateGrid();
            }
        }

        EditorGUILayout.HelpBox("Point amount: " + grid.points.Count, MessageType.Info);
    }
}
