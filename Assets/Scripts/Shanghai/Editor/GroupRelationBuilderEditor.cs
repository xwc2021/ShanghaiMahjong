using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(GroupRelationBuilder))]
public class GroupRelationBuilderEditor : UnityEditor.Editor
{
    GroupRelationBuilder groupRelationBuilder;
    void OnEnable()
    {
        groupRelationBuilder = (GroupRelationBuilder)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Build Groups & Links"))
        {
            groupRelationBuilder.BuildGroups();
            groupRelationBuilder.BuildLinks();
            SceneView.RepaintAll();
        }
        if (GUILayout.Button("BuildDependence"))
        {
            groupRelationBuilder.BuildDependence();
            SceneView.RepaintAll();
        }
        

        if (GUILayout.Button("Up"))
        {
            groupRelationBuilder.SetNowFloorIndex(1);
            SceneView.RepaintAll();
        }

        if (GUILayout.Button("Down"))
        {
            groupRelationBuilder.SetNowFloorIndex(-1);
            SceneView.RepaintAll();
        }
    }
}