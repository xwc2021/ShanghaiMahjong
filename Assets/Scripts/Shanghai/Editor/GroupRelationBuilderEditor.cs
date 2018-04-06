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
        Vector3 from, clickWorldPoint, normalDir;
        GeometryTool.GetShootingRay(mousePos, out from, out clickWorldPoint, out normalDir);

        var voxelBuilder = groupRelationBuilder.GetVoxelBuilder();
        //假裝自己是在對voxelBuilder發射
        var localFrom = groupRelationBuilder.transform.InverseTransformPoint(from);

        int nowFloor, nowY, nowX;
        bool isHit = voxelBuilder.DoClick(localFrom, normalDir, out nowFloor, out nowY, out nowX,true);
        if (!isHit)
            return;

        var selectGroup=voxelBuilder.GetVoxel(nowFloor, nowY, nowX).group;
        groupRelationBuilder.nowSelectGroup = selectGroup;
    }
}