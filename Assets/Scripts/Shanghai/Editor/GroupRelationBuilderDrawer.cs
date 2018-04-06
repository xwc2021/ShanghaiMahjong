using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class GroupRelationBuilderDrawer
{
    static Color downToUpLinkColor = Color.black;
    static Color GroupRelationColor = Color.white;

    static int randomColorIndex;
    static Color[] randomColors = new Color[] { Color.red, Color.green, Color.blue };
    static Color singleColor = Color.black;
    static Color SelectGroupColor = Color.yellow;

    [DrawGizmo(GizmoType.Selected | GizmoType.Active)]
    static void DrawGizmoFor(GroupRelationBuilder target, GizmoType gizmoType)
    {
        var nowSelectGroup=target.GetNowSelectGroup();
        if (!target.showTotalFloor) {
            DrawGroupAndGroupLinks(nowSelectGroup, target.GetGroupList(), target.GetGroupLinks(),GetSingleColor);
        }
        else
        {
            var downToUpLinks= target.GetDownToUpLinks();
            foreach (var link in downToUpLinks)
                DrawElementLink(link, downToUpLinkColor,Vector3.forward);

            ResetRandomColorIndex();
            DrawGroupAndGroupLinks(nowSelectGroup, target.GetTotalGroupList(), target.GetTotalGroupLinks(),GetRandomColor);
        }
    }

    static void DrawGroupAndGroupLinks(Group nowSelectGroup ,List<Group> groups,List<GroupRelation> groupLinks, GetColor getColorFunc)
    {
        foreach (var g in groups)
            GroupDrawer.DrawGroup(g, getColorFunc());

        if(nowSelectGroup!=null)
            GroupDrawer.DrawGroup(nowSelectGroup, SelectGroupColor);

        foreach (var link in groupLinks)
            DrawElementLink(link.GetElementRelation(), GroupRelationColor,Vector3.up);
    }

    
    delegate Color GetColor();
    static Color GetSingleColor() { return singleColor; }
    static Color GetRandomColor() {
        randomColorIndex = (randomColorIndex + 1) % randomColors.Length;
        return randomColors[randomColorIndex];
    }
    static void ResetRandomColorIndex() { randomColorIndex = -1; }
    
    static void DrawElementLink(ElementRelation link,Color color,Vector3 help) {
        Gizmos.color = color;
        var triggerPos = link.GetTrigger().transform.position;
        var waitingPos = link.GetWaiting().transform.position;
        var centerPos = 0.5f * triggerPos + 0.5f * waitingPos;
        var traceBack =triggerPos - waitingPos;


        Gizmos.DrawSphere(triggerPos, 0.1f);
        Gizmos.DrawLine(triggerPos, waitingPos);
        Gizmos.DrawLine(centerPos, GetBackPoint(centerPos, traceBack,help, arrowDegree, arrowLength));
        Gizmos.DrawLine(centerPos, GetBackPoint(centerPos, traceBack, help, -arrowDegree, arrowLength));
    }

    static float arrowDegree = 45;
    static float arrowLength = 0.1f;
    static Vector3 GetBackPoint(Vector3 point,Vector3 X, Vector3 help,float degree,float distance)
    {
        var rad = degree * Mathf.Deg2Rad;
        var Z = Vector3.Cross(X, help);
        return point+X * distance * Mathf.Cos(rad) + Z * distance * Mathf.Sin(rad);
    }
}