using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeometryTool  {

    static public bool RayHitPlane(Vector3 from, Vector3 dir, Vector3 PlaneN, Vector3 PlaneC, out Vector3 hitPos)
    {
        //(F-C)。N + t (D。N) = 0
        // t  = (C-F)。N / (D。N)
        // t  = (A / (B)
        var B = Vector3.Dot(dir, PlaneN);
        var A = Vector3.Dot(PlaneC- from, PlaneN);

        var Epsilon = 0.0001f;
        if (Mathf.Abs(B) < Epsilon)
        {
            hitPos = Vector3.zero;
            return false;
        }  
        var t = A / B;
        hitPos = from + t * dir;
        return true;
    }
}
