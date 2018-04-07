using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GroupState {
    ShuffleNotUsing,//還沒放置過
    ShuffleUsing,//至少有1個被放置了
    GameReady,//整條都洗牌完了
    GameFinish//玩家已經清掉整條
}
public class Group : MonoBehaviour
{
    public void CheckIfHasFloorLink()
    {
        foreach (var e in elements) {
            if (e.triggerCount > 0) {
                hasFloorLink = true;
                break;
            }
        }
        hasFloorLink = false;
    }
    public bool hasFloorLink;
    public bool hasGroupRelation;
    public bool isSuffleList;
    public GroupRelationBuilder groupRelationBuilder;

    public float GetPosX() { return transform.position.x; }
    public Element GetHeadElement() { return elements[0]; }
    public Element GetTailElement() { return elements[elements.Length-1]; }

    public GroupState state;
    public bool hasDependence = false;

    bool isFirstSuffle;//是不是一開局的洗牌？
    int inGameLeftIndex, inGameRightIndex;//記錄遊戲進行中的左右2端
    int shuffleLeftIndex, shuffleRightIndex;
    int shuffeCount;

    [SerializeField]
    Element[] elements;
    public void AddElements(Element[] element){
        elements = element;
        foreach (var e in element)
            e.group = this;
    }

    public void AddToShufflingSet()
    {
        groupRelationBuilder.AddToShufflingSet(this);
    }

    public Element[] GetElements(){return elements;}

    public int xBegin,xEnd;
    public int y;
    public int floor;
    public void Set(int floor, int y, int xBegin, int xEnd)
    {
        this.floor = floor;
        this.y = y;
        this.xBegin = xBegin;
        this.xEnd = xEnd;
        name = floor + "," + y + ", [" + xBegin + " to "  + xEnd+"]";
    }

    public void Setpos(Voxel voxel)
    { transform.localPosition = voxel.transform.localPosition; }

    public void PickOne()
    {
        //如果elment裡有outputTrigger就發送
    }

    public Vector3[] GetRect() {
        var BeginElement = elements[0].transform;
        var EndElement = elements[elements.Length-1].transform;
        var offsetX = 0.5f*VoxelBuilder.xUnit*Vector3.right;
        var offsetY = 0.5f * VoxelBuilder.yUnit * Vector3.forward;
        return new Vector3[] {//從左下角開始逆時鐘方向轉1圈
            BeginElement.position- offsetX- offsetY,
            EndElement.position+ offsetX- offsetY,
            EndElement.position+ offsetX+ offsetY,
            BeginElement.position- offsetX+ offsetY
        };
    }

    int GetSuffleMaxCount()
    {
        if (isFirstSuffle)
            return elements.Length;
        else
            return inGameRightIndex - inGameLeftIndex + 1;
    }

    public bool IsSuffleFinish() { return shuffeCount == GetSuffleMaxCount(); }

    public void SetIsFirstShuffle(bool b){ isFirstSuffle = b; }

    public void BeforeShuffle()
    {
        isSuffleList = false;
        shuffeCount = 0;
        state = GroupState.ShuffleNotUsing;
    }
}
