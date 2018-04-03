using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNode : MonoBehaviour {

    public static float HitRangeRadius = 0.15f;

    [SerializeField]
    bool isUse = false;

    public bool IsUse() { return isUse; }
    public void SetIsUse(bool b) { isUse = b; }

    public bool IsHit(Vector3 hitPos) {
        var distance = (hitPos - transform.position).magnitude;
        return distance < HitRangeRadius;
    }
}
