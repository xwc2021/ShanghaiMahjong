using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(VoxelBuilder))]
public class VoxelBuilderEditor : UnityEditor.Editor
{
    VoxelBuilder voxelBuilder;
    void OnEnable()
    {
        voxelBuilder = (VoxelBuilder)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("generateMap"))
        {
            voxelBuilder.SyncPos();
            voxelBuilder.GenerateMap();
            SceneView.RepaintAll();
        }

        if (GUILayout.Button("Up"))
        {
            voxelBuilder.SetNowFloorIndex(1);
            SceneView.RepaintAll();
        }

        if (GUILayout.Button("Down"))
        {
            voxelBuilder.SetNowFloorIndex(-1);
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
        Vector3 from, clickWorldPoint, normalDir;
        if(camera.orthographic)
            GetRayOrthographic(mousePos,out from, out clickWorldPoint, out normalDir);
        else
            GetRay(mousePos, out from, out clickWorldPoint, out normalDir);

        //debug用
        voxelBuilder.SetClickPointOnRay(clickWorldPoint);
        voxelBuilder.SetClickNormalDir(normalDir);

        int nowFloor, nowY, nowX;
        //mahjongMap的平面朝向是固定的，所以不作座標變換
        bool isHit =voxelBuilder.DoClick(from, normalDir,out nowFloor, out nowY, out nowX);
        if (!isHit)
            return;

        var funptr = DoWhat();
        for (var y = 0; y < voxelBuilder.GetAddCountY(); ++y) {
            for (var x = 0; x < voxelBuilder.GetAddCountX(); ++x){
                var node = voxelBuilder.GetVoxel(nowFloor, nowY+2*y, nowX+ 2 * x);
                if (node == null)
                    continue;
   
                voxelBuilder.DoOperation(node,funptr);
            }
        }
        
    }

    VoxelBuilder.FuncPtr DoWhat()
    {
        VoxelBuilder.FuncPtr funptr;
        switch (voxelBuilder.GetOperation())
        {
            case EditOperation.Use:
                funptr = voxelBuilder.UseNode;
                break;
            case EditOperation.NotUse:
                funptr = voxelBuilder.NotUseNode;
                break;
            default:
                funptr = voxelBuilder.ReverseNode;
                break;
        }
        return funptr;
    }

    void GetRay(Vector3 mousePos, out Vector3 from, out Vector3 clickWorldPoint, out Vector3  normalDir) {
        var camera = Camera.current;
        mousePos.y = camera.pixelHeight - mousePos.y;//mousePos左上角是(0,0)
        from = camera.transform.position;
        //第3個參數的距離沿著camera forward的方向
        clickWorldPoint = camera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, voxelBuilder.GetClickPointDistance()));

        normalDir = (clickWorldPoint - from).normalized;
    }

    //正交camera
    void GetRayOrthographic(Vector3 mousePos, out Vector3 from, out Vector3 clickWorldPoint, out Vector3 normalDir)
    {
        var camera = Camera.current;
        mousePos.y = camera.pixelHeight - mousePos.y;//mousePos左上角是(0,0)

        var halfWidth = 0.5f * camera.pixelWidth;
        var halfHeight = 0.5f * camera.pixelHeight;
        var nx = (mousePos.x- halfWidth) / halfWidth;
        var ny = (mousePos.y- halfHeight) / halfHeight;
        var ratio = (float)camera.pixelWidth / camera.pixelHeight;

        var offsetY = ny * camera.orthographicSize;
        var offsetX = ratio*nx * camera.orthographicSize;
        from = camera.transform.position+
            offsetY* camera.transform.up +
            offsetX* camera.transform.right;
        clickWorldPoint = from+ camera.transform.forward*voxelBuilder.GetClickPointDistance();

        normalDir = camera.transform.forward;
    }
}