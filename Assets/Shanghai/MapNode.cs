using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNode : MonoBehaviour {

    public static float HitRangeRadius = 0.2f;

    [SerializeField]
    MeshRenderer meshRenderer;

    [SerializeField]
    bool isUse = false;

    public bool IsUse() { return isUse; }
    public void SetIsUse(bool b) { isUse = b; meshRenderer.enabled = b; }

    public int x;
    public int y;
    public int floor;
    public void Init(int floor, int y,int x)
    {
        this.floor = floor;
        this.y = y;
        this.x = x;
        name = floor + "," + y + "," + x;
        meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    public bool IsHit(Vector3 hitPos) {
        var distance = (hitPos - transform.position).magnitude;
        return distance < HitRangeRadius;
    }
}
