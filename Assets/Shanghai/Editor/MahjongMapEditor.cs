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
    }

    public void OnSceneGUI()
    {
        if (Event.current.type == EventType.MouseDown)
        {
            if (Event.current.button == 1)//right button
            {
                ShootRay(Event.current.mousePosition);
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

        //因為mahjongMap的原點在(0,0,0)也沒有旋轉，所以不作座標變換
        mahjongMap.AddOne(from, normalDir);
    }
}