using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class GroupRelationBuilderDrawer
{
    static Color downToUpLinkColor = Color.black;
    static Color GroupRelationColor = Color.white;

    static int colorIndex;
    static Color[] colors = new Color[] { Color.red, Color.green, Color.blue, Color.yellow };
    static Color singleColor = Color.black;
    static Color SelectGroupColor = Color.yellow;

    [DrawGizmo(GizmoType.Selected | GizmoType.Active)]
    static void DrawGizmoFor(GroupRelationBuilder target, GizmoType gizmoType)
    {
        if (!target.showTotalFloor) {
            DrawGroupAndGroupLinks(target.nowSelectGroup,target.GetGroupList(), target.GetGroupLinks(),GetSingleColor);
        }
        else
        {
            var downToUpLinks= target.GetDownToUpLinks();
            foreach (var link in downToUpLinks)
                DrawElementLink(link, downToUpLinkColor);

            DrawGroupAndGroupLinks(target.nowSelectGroup, target.GetTotalGroupList(), target.GetTotalGroupLinks(),GetRandomColor);
        }
    }

    static void DrawGroupAndGroupLinks(Group nowSelectGroup ,List<Group> groups,List<GroupRelation> groupLinks, GetColor getColorFunc)
    {
        foreach (var g in groups)
            GroupDrawer.DrawGroup(g, getColorFunc());
        GroupDrawer.DrawGroup(nowSelectGroup, SelectGroupColor);

        foreach (var link in groupLinks)
            DrawElementLink(link.GetElementRelation(), GroupRelationColor);
    }

    
    delegate Color GetColor();
    static Color GetSingleColor() { return singleColor; }
    static Color GetRandomColor() {
        colorIndex = (colorIndex + 1) % colors.Length;
        return colors[colorIndex];
    }
    
    static void DrawElementLink(ElementRelation link,Color color) {
        Gizmos.color = color;
        var triggerPos = link.GetTrigger().transform.position;
        var waitingPos = link.GetWaiting().transform.position;
        Gizmos.DrawLine(triggerPos, waitingPos);
        Gizmos.DrawSphere(triggerPos, 0.1f);
    }
}