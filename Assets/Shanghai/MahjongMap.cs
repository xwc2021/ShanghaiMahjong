using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MahjongMap : MonoBehaviour {

    public static float xUnit = 1.0f;
    public static float yUnit = 1.5f;
    public static float heightUnit = 0.5f;

    [SerializeField]
    MapNode mapNodePrefab;

    [SerializeField]
    private MapNode[] map3D;

    public int CountY() { return 2 * Y - 1; }
    public int CountX() { return 2 * X - 1; }
    public void GenerateMap()
    {
        var count =transform.childCount;
        for (var i = count - 1; i >= 0; --i)
            DestroyImmediate(transform.GetChild(i).gameObject);

        Debug.Log("clear count = " + count);

        map3D = new MapNode[Floor* CountY()* CountX()];
        var original = transform.position;
        var offsetX = 0.5f * Vector3.right * MahjongMap.xUnit;
        var offsetY = 0.5f * Vector3.forward * MahjongMap.yUnit;
        var offsetFloor =  Vector3.up * MahjongMap.heightUnit;
        var offsetXY = offsetX + offsetY;
        for (var f = 0; f < Floor; ++f) {
            for (var y = 0; y < CountY(); ++y){
                for (var x = 0; x < CountX(); ++x){
                    var node =Instantiate<MapNode>(mapNodePrefab);
                    node.transform.position = original + offsetXY + offsetFloor * f + offsetY * y + offsetX * x;
                    node.transform.parent = this.transform;
                    node.Init(f, y, x);
                    var index = ReMap(f, y, x);
                    map3D[index] = node;
                }
            }
        }
    }


    int ReMap(int floorIndex, int y, int x) {
        return floorIndex * CountY() * CountX() + y *CountX() + x;
    }

    public bool IsSetValue(int floorIndex, int y,int x)
    {
        if (map3D == null)
            return false;

        var index = ReMap(floorIndex, y, x);
        return map3D[index].IsUse();
    }

    [SerializeField]
    int X=10;
    [SerializeField]
    int Y=10;//Z方向定義成Y
    [SerializeField]
    int Floor = 10;//Y方向定義成Height

    [SerializeField]
    int nowFloorIndex = 0;
    public int GetNowFloorIndex() { return nowFloorIndex; }
    public void SetNowFloorIndex(int offset) {
        var newIndex = nowFloorIndex + offset;
        if (IsValidatedFloorIndex(newIndex))
            nowFloorIndex = newIndex;
    }
    public bool IsValidatedFloorIndex(int index) { return index >= 0 && index < Floor; }
    public bool IsValidatedX(int x) { return x >= 0 && x < CountX(); }
    public bool IsValidatedY(int y) { return y >= 0 && y < CountY(); }

    MapNode GetNode(int floor, int y, int x) {
        if (IsValidatedY(y) && IsValidatedX(x))
        {
            var index = ReMap(floor, y, x);
            return map3D[index];
        }
        return null;
    }

    public int GetX() { return X; }
    public int GetY() { return Y; }
    public int GetAllFloor() { return Floor; }
    public Vector3 GetNowFlowerHeight() { return Vector3.up * MahjongMap.heightUnit * nowFloorIndex; }

    Vector3 clickPointOnRay;
    public Vector3 GetClickPointOnRay() { return clickPointOnRay; }
    public void SetClickPointOnRay(Vector3 p) { clickPointOnRay = p; }

    Vector3 clickNormalDir;
    public Vector3 GetClickNormalDir() { return clickNormalDir; }
    public void SetClickNormalDir(Vector3 normalDir) { clickNormalDir = normalDir; }

    [SerializeField]
    int clickPointDistance = 10;
    public int GetClickPointDistance() { return clickPointDistance; }

    Vector3 hitPoint;
    public Vector3 GetHitPoint() { return hitPoint; }

    public bool IsCanUse(MapNode node) {
        var x = node.x;
        var y = node.y;
        var f = node.floor;

        //8個角都沒在使用才行
        var node8 = new MapNode[] { GetNode(f, y, x-1) ,GetNode(f, y, x+1) ,
                                    GetNode(f, y-1, x) ,GetNode(f, y+1, x) ,
                                    GetNode(f, y-1, x-1) ,
                                    GetNode(f, y+1, x+1) ,
                                    GetNode(f, y-1, x+1) ,
                                    GetNode(f, y+1, x-1) } ;
        for(var i = 0; i < node8.Length; ++i)
        {
            var nowNode = node8[i];
            if (nowNode == null)
                continue;

            if (nowNode.IsUse())
                return false;
        }
        return true;
    }

    public void DoClick(Vector3 from ,Vector3 dir)
    {
        bool hit = GeometryTool.RayHitPlane(from, dir, Vector3.up, transform.position+ GetNowFlowerHeight(), out hitPoint);
        if (!hit)
            return;

        var node=GetMapNode();
        if (node == null)
            return;

        bool hitSphere = node.IsHit(hitPoint);
        if (!hitSphere)
            return;

        bool canUse = IsCanUse(node);
        if (!canUse)
            return;

        if (node.IsUse())
            node.SetIsUse(false);
        else
            node.SetIsUse(true);
    }

    MapNode GetMapNode()
    {
        var offsetX = 0.25f * Vector3.right * MahjongMap.xUnit;
        var offsetY = 0.25f * Vector3.forward * MahjongMap.yUnit;
        var refPoint = transform.position + offsetX + offsetY;

        var diff = (hitPoint - refPoint);
        var halfXUnit = 0.5 * MahjongMap.xUnit;
        var halfYUnit = 0.5 * MahjongMap.yUnit;
        var x = (int)((diff.x-(diff.x % halfXUnit))/ halfXUnit);
        var y = (int)((diff.z-(diff.z % halfYUnit))/ halfYUnit);

        return GetNode(nowFloorIndex, y, x);
    }
}
