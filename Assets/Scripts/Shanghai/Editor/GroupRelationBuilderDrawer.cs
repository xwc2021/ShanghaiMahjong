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
            GroupDrawer.DrawGroup(g, GetColor());
    }
    static int colorIndex;
    static Color[] colors=new Color[] {Color.red,Color.green,Color.blue,Color.yellow };
    static Color GetColor() {
        colorIndex = (colorIndex + 1) % colors.Length;
        return colors[colorIndex];
    }
}