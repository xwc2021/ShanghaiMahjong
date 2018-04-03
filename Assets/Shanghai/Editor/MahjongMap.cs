using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(MahjongMap))]
public class MahjongMapEditor : UnityEditor.Editor
{
    MahjongMap mahjongMap;
    void OnEnable()
    {
        mahjongMap = (MahjongMap)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("show"))
        {
   
            SceneView.RepaintAll();
        }

        if (GUILayout.Button("hide"))
        {
   
        }
    }
}