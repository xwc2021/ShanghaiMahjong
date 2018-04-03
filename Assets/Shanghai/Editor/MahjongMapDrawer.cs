using UnityEngine;
using System.Collections;
using UnityEditor;

public class MahjongMapDrawer
{
    static Color EvenPointColor = Color.blue;
    static Color OddPointColor = Color.green;

    [DrawGizmo(GizmoType.Selected | GizmoType.Active)]
    static void DrawGizmoFor(MahjongMap target, GizmoType gizmoType)
    {
        var X = target.GetX(); var Y = target.GetY();var nowHeight = target.GetNowFlowerHeight();

        var original = target.transform.position + nowHeight;

        //畫直線
        var yLine = Y * MahjongMap.yUnit * Vector3.forward;
        var offset = MahjongMap.xUnit * Vector3.right;
        var from = original;
        for (var x = 1; x <= X + 1; ++x) {
            Gizmos.DrawLine(from, from + yLine);
            from = from + offset;
        }

        //畫橫線
        offset = MahjongMap.yUnit * Vector3.forward;
        var xLine = X * MahjongMap.xUnit * Vector3.right; ;
        from = original;
        for (var y = 1; y <= Y + 1; ++y){
            Gizmos.DrawLine(from, from + xLine);
            from = from + offset;
        }

        //畫點
        var offsetX = 0.5f * Vector3.right * MahjongMap.xUnit;
        var offsetY = 0.5f * Vector3.forward * MahjongMap.yUnit;
        var offsetXY = offsetX + offsetY;
        var r = 0.1f;
        var borderY = 2 * Y-1; var borderX = 2 * X-1;
        for (var y = 0; y < borderY; ++y){
            from = original + offsetXY + offsetY*y;
            for (var x = 0; x < borderX; ++x){

                if(x%2==0 && y%2==0)
                    Gizmos.color = EvenPointColor;
                else
                    Gizmos.color = OddPointColor;

                Gizmos.DrawSphere(from, r);
                from = from + offsetX;
            }
        }
    }
}