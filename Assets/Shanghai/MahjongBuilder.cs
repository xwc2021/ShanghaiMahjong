using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EditOperation { Use,NotUse, Reverse }

public class MahjongBuilder : MonoBehaviour {

    public static float xUnit = 1.0f;
    public static float yUnit = 1.5f;
    public static float heightUnit = 0.5f;

    [SerializeField]
    MahjongTemplate mahjongTemplate;

    [SerializeField]
    EditOperation operation= EditOperation.Use;
    public EditOperation GetOperation() { return operation; }

    public delegate void FuncPtr(Mahjong node);
    public void DoOperation(Mahjong node, FuncPtr func) {
        func(node);
    }

    [SerializeField]
    int addCountX = 1;

    [SerializeField]
    int addCountY = 1;

    public int GetAddCountX() { return addCountX; }
    public int GetAddCountY() { return addCountY; }

    [SerializeField]
    GameObject mahjongOdd;
    [SerializeField]
    GameObject mahjongEven;
    [SerializeField]
    Mahjong mahjong;

    [SerializeField]
    [HideInInspector]
    private Mahjong[] map3D;

    public int CountY() { return 2 * Y - 1; }
    public int CountX() { return 2 * X - 1; }

    public void SyncPos()
    {
        mahjongTemplate.SyncPos(transform.position);
    }

    public void GenerateMap()
    {
        nowFloorIndex = 0;
        Tool.Clear(transform);
        mahjongTemplate.Clear();

        map3D = new Mahjong[Floor* CountY()* CountX()];
        var original = transform.position;
        var offsetX = 0.5f * Vector3.right * MahjongBuilder.xUnit;
        var offsetY = 0.5f * Vector3.forward * MahjongBuilder.yUnit;
        var offsetFloor =  Vector3.up * MahjongBuilder.heightUnit;
        var offsetXY = offsetX + offsetY;
        for (var f = 0; f < Floor; ++f) {
            bool isOdd = f % 2 == 0;
            for (var y = 0; y < CountY(); ++y){
                for (var x = 0; x < CountX(); ++x){
                    var node =Instantiate<Mahjong>(mahjong);
                    node.transform.position = original + offsetXY + offsetFloor * f + offsetY * y + offsetX * x;
                    node.transform.parent = transform;
                    node.Init(f, y, x, isOdd );
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

    public Mahjong GetNode(int floor, int y, int x) {
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
    public Vector3 GetNowFlowerHeight() { return Vector3.up * MahjongBuilder.heightUnit * nowFloorIndex; }

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

    public bool IsCanUse(Mahjong node) {
        var x = node.x;
        var y = node.y;
        var f = node.floor;

        //8個角都沒在使用才行
        var node8 = new Mahjong[] { GetNode(f, y, x-1) ,GetNode(f, y, x+1) ,
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

    public bool DoClick(Vector3 from ,Vector3 dir,out int floor,out int y,out int x)
    {
        floor = -1;
        x = -1;
        y = -1;

        bool hit = GeometryTool.RayHitPlane(from, dir, Vector3.up, transform.position+ GetNowFlowerHeight(), out hitPoint);
        if (!hit)
            return false;

        var node=GetMapNode();
        if (node == null)
            return false;

        bool hitSphere = node.IsHit(hitPoint);
        if (!hitSphere)
            return false;

        x = node.x;
        y = node.y;
        floor = node.floor;

        return true;
    }

    public void ReverseNode(Mahjong node)
    {
        bool canUse = IsCanUse(node);
        if (!canUse)
            return;

        if (node.IsUse())
        {
            node.SetIsUse(false);
            RemoveVisible(node);
        }
        else
        {
            node.SetIsUse(true);
            AddVisible(node);
        }
            
    }

    public void UseNode(Mahjong node)
    {
        bool canUse = IsCanUse(node);
        if (!canUse)
            return;

        node.SetIsUse(true);
        AddVisible(node);
    }

    public void NotUseNode(Mahjong node)
    {
        bool canUse = IsCanUse(node);
        if (!canUse)
            return;

        node.SetIsUse(false);
        RemoveVisible(node);
    }

    void AddVisible(Mahjong node)
    {
        if (node.visible != null)
            return;

        var obj =Instantiate<GameObject>(node.IsOdd() ? mahjongOdd : mahjongEven,mahjongTemplate.transform);
        obj.transform.localPosition = node.transform.localPosition;
        obj.name = node.name;
        node.visible = obj;
    }

    void RemoveVisible(Mahjong node)
    {
        DestroyImmediate(node.visible);
        node.visible = null;
    }

    Mahjong GetMapNode()
    {
        var offsetX = 0.25f * Vector3.right * MahjongBuilder.xUnit;
        var offsetY = 0.25f * Vector3.forward * MahjongBuilder.yUnit;
        var refPoint = transform.position + offsetX + offsetY;

        var diff = (hitPoint - refPoint);
        var halfXUnit = 0.5 * MahjongBuilder.xUnit;
        var halfYUnit = 0.5 * MahjongBuilder.yUnit;
        var x = (int)((diff.x-(diff.x % halfXUnit))/ halfXUnit);
        var y = (int)((diff.z-(diff.z % halfYUnit))/ halfYUnit);

        return GetNode(nowFloorIndex, y, x);
    }
}
