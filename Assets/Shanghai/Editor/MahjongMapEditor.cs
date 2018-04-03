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

        if (GUILayout.Button("generateMap"))
        {
            mahjongMap.GenerateMap();
            SceneView.RepaintAll();
        }

        if (GUILayout.Button("Up"))
        {
            mahjongMap.SetNowFloorIndex(1);
            SceneView.RepaintAll();
        }

        if (GUILayout.Button("Down"))
        {
            mahjongMap.SetNowFloorIndex(-1);
            SceneView.RepaintAll();
        }
    }

    public void OnSceneGUI()
    {
        if (Event.current.type == EventType.MouseDown)
        {
            if (Event.current.button == 1)//right button
            {
                ShootRay(Event.current.mousePosition);
                SceneView.RepaintAll();
            }
        }
    }

    void ShootRay(Vector3 mousePos)
    {
        var camera = Camera.current;
        mousePos.y = camera.pixelHeight - mousePos.y;//mousePos左上角是(0,0)
        var clickWorldPoint = camera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, mahjongMap.GetClickPointDistance()));
        var from = camera.transform.position;
        var normalDir = (clickWorldPoint- from).normalized;

        //debug用
        mahjongMap.SetClickPointOnRay(clickWorldPoint);
        mahjongMap.SetClickNormalDir(normalDir);

        //mahjongMap的平面朝向是固定的，所以不作座標變換
        mahjongMap.DoClick(from, normalDir);
    }
}