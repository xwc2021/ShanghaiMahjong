using UnityEngine;
using System.Collections;
using UnityEditor;

public class ElementDrawer
{
    [DrawGizmo(GizmoType.Selected | GizmoType.Active)]
    static void DrawGizmoFor(Element target, GizmoType gizmoType)
    {
        DrawGroup(target, Color.yellow);
    }
    static void DrawGroup(Element element, Color color)
    {
        Gizmos.color = color;
        var points = element.GetRect();
        Gizmos.DrawLine(points[0], points[1]);
        Gizmos.DrawLine(points[1], points[2]);
        Gizmos.DrawLine(points[2], points[3]);
        Gizmos.DrawLine(points[3], points[0]);
    }
}