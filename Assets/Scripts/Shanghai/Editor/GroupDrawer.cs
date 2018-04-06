using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class GroupDrawer
{
    [DrawGizmo(GizmoType.Selected | GizmoType.Active)]
    static void DrawGizmoFor(Group target, GizmoType gizmoType)
    {
        DrawGroup(target,Color.yellow);
    }
    public static void DrawGroup(Group group, Color color)
    {
        Gizmos.color = color;
        var points = group.GetRect();
        Gizmos.DrawLine(points[0], points[1]);
        Gizmos.DrawLine(points[1], points[2]);
        Gizmos.DrawLine(points[2], points[3]);
        Gizmos.DrawLine(points[3], points[0]);
        Gizmos.DrawSphere(group.transform.position, 0.2f);
    }
}