using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(Game))]
public class GameEditor : UnityEditor.Editor
{
    Game game;
    void OnEnable()
    {
        game = (Game)target;
    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("ShuffleOneStep"))
        {
            game.ShuffleOneStep();
            SceneView.RepaintAll();
        }
    }
}