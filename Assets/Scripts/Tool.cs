using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool  {
    public static void Clear(Transform transform)
    {
        var count = transform.childCount;
        for (var i = count - 1; i >= 0; --i)
            GameObject.DestroyImmediate(transform.GetChild(i).gameObject);
        Debug.Log("clear count = " + count);
    }
}
