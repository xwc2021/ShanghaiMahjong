using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voxel : MonoBehaviour {

    public static float HitRangeRadius = 0.2f;

    [SerializeField]
    bool isUse = false;
    public bool IsUse() { return isUse; }
    public void SetIsUse(bool b) { isUse = b; }

    [SerializeField]
    bool isOdd = true;
    public bool IsOdd() { return isOdd; }

    public int x;
    public int y;
    public int floor;
    public GameObject visible;
    public void Init(int floor, int y,int x,bool isOdd)
    {
        this.floor = floor;
        this.y = y;
        this.x = x;
        this.isOdd = isOdd;
        name = floor + "," + y + "," + x;
    }

    public bool IsHit(Vector3 hitPos) {
        var distance = (hitPos - transform.position).magnitude;
        return distance < HitRangeRadius;
    }
}
