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

        if (GUILayout.Button("Build"))
        {
            groupRelationBuilder.Build();
            SceneView.RepaintAll();
        }
    }
}