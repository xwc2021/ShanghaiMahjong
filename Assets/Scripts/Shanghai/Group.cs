using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GroupState {
    ShuffleNotUsing,//還沒被入ShufflingList
    ShuffleUsing,//放入ShufflingList
    GameReady,//整條都洗牌完了
    GameFinish//玩家已經清掉整條
}
public class Group : MonoBehaviour {

    public GroupState state;
    public bool hasDependence = false;

    int leftBorderIndex, rightBorderIndex,nowIndex;

    [SerializeField]
    Element[] elements;
    public void AddElements(Element[] element){elements = element;}
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
}
