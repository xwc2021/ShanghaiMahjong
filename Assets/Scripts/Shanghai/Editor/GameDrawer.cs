using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class GameDrawer
{
    [DrawGizmo(GizmoType.Selected | GizmoType.Active)]
    static void DrawGizmoFor(Game target, GizmoType gizmoType)
    {
        foreach (var g in target.shufflingList)
            GroupDrawer.DrawGroup(g, Color.yellow);
    }
}