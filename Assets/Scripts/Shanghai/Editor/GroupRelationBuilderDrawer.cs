using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class GroupRelationBuilderDrawer
{
    [DrawGizmo(GizmoType.Selected | GizmoType.Active)]
    static void DrawGizmoFor(GroupRelationBuilder target, GizmoType gizmoType)
    {
        var groups =target.GetGroupList();
        colorIndex = -1;
        foreach (var g in groups)
            GroupDrawer.DrawGroup(g, GetColor());

        var groupLinks =target.GetGroupLinks();
        foreach (var link in groupLinks)
            DrawGroupLink(link.GetElementRelation());
    }
    static int colorIndex;
    static Color[] colors=new Color[] {Color.red,Color.green,Color.blue,Color.yellow };
    static Color GetColor() {
        colorIndex = (colorIndex + 1) % colors.Length;
        return colors[colorIndex];
    }
    static Color GroupRelationColor = Color.white;
    static void DrawGroupLink(ElementRelation link) {

        Gizmos.color = GroupRelationColor;
        var triggerPos = link.GetTrigger().transform.position;
        var waitingPos = link.GetWaiting().transform.position;
        Gizmos.DrawLine(triggerPos, waitingPos);
        Gizmos.DrawSphere(triggerPos, 0.1f);
    }
}