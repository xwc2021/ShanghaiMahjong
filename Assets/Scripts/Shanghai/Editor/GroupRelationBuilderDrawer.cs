using UnityEngine;
using System.Collections;
using UnityEditor;

public class GroupRelationBuilderDrawer
{
    [DrawGizmo(GizmoType.Selected | GizmoType.Active)]
    static void DrawGizmoFor(GroupRelationBuilder target, GizmoType gizmoType)
    {
        var groups =target.GetGroupList();
        colorIndex = -1;
        foreach (var g in groups)
            DrawGroup(g);
    }
    static int colorIndex;
    static Color[] colors=new Color[] {Color.red,Color.green,Color.blue,Color.yellow };
    static Color GetColor() {
        colorIndex = (colorIndex + 1) % colors.Length;
        return colors[colorIndex];
    }
    static void DrawGroup(Group group)
    {
        var points = group.GetRect();
        Gizmos.color = GetColor();
        Gizmos.DrawLine(points[0], points[1]);
        Gizmos.DrawLine(points[1], points[2]);
        Gizmos.DrawLine(points[2], points[3]);
        Gizmos.DrawLine(points[3], points[0]);
    }
}