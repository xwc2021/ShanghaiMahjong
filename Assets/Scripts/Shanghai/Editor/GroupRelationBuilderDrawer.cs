using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class GroupRelationBuilderDrawer
{
    [DrawGizmo(GizmoType.Selected | GizmoType.Active)]
    static void DrawGizmoFor(GroupRelationBuilder target, GizmoType gizmoType)
    {
        if (!target.showTotalFloor) {
            DrawGroupAndGroupLinks(target.GetGroupList(), target.GetGroupLinks());
        }
        else
        {
            var downToUpLinks= target.GetDownToUpLinks();
            foreach (var link in downToUpLinks)
                DrawElementLink(link, downToUpLinkColor);

            DrawGroupAndGroupLinks(target.GetTotalGroupList(), target.GetTotalGroupLinks());
        }
    }

    static void DrawGroupAndGroupLinks(List<Group> groups,List<GroupRelation> groupLinks)
    {
        colorIndex = -1;
        foreach (var g in groups)
            GroupDrawer.DrawGroup(g, GetColor());

        foreach (var link in groupLinks)
            DrawElementLink(link.GetElementRelation(), GroupRelationColor);
    }

    static int colorIndex;
    static Color[] colors=new Color[] {Color.red,Color.green,Color.blue,Color.yellow };
    static Color GetColor() {
        colorIndex = (colorIndex + 1) % colors.Length;
        return colors[colorIndex];
    }
    static Color  downToUpLinkColor= Color.black;
    static Color GroupRelationColor = Color.white;
    static void DrawElementLink(ElementRelation link,Color color) {
        Gizmos.color = color;
        var triggerPos = link.GetTrigger().transform.position;
        var waitingPos = link.GetWaiting().transform.position;
        Gizmos.DrawLine(triggerPos, waitingPos);
        Gizmos.DrawSphere(triggerPos, 0.1f);
    }
}