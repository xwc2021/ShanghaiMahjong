using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//前3個是洗牌狀態，最後1個是玩家刪除
public enum ElementState {
    ShuffleWaitTriggerMsg,//等待所有trigger都滿足才能被配置
    ShuffleCanUse,//可以被配置
    ShuffleUsed,//已經被配置
    Delete//玩家已經消除
}
public class Element : MonoBehaviour {

    public Group group;
    public ElementState state;
    public int floor;
    public int y;
    public int x;

    public void Set(Voxel voxel)
    {
        transform.localPosition = voxel.transform.localPosition;
        name = voxel.floor+","+ voxel.y+ "," + voxel.x;
        this.floor = voxel.floor;
        this.y = voxel.y;
        this.x = voxel.x;
    }

    public void BeforeSuffle() {

    }

    public void DoReceive()
    {
        //所有trigger達成
        if (msgCount == triggerCount)
            group.AddToShufflingSet();
    }

    public Vector3[] GetRect()
    {
        var center = transform.position;
        var offsetX = 0.5f * VoxelBuilder.xUnit * Vector3.right;
        var offsetY = 0.5f * VoxelBuilder.yUnit * Vector3.forward;
        return new Vector3[] {//從左下角開始逆時鐘方向轉1圈
            center- offsetX- offsetY,
            center+ offsetX- offsetY,
            center+ offsetX+ offsetY,
            center- offsetX+ offsetY
        };
    }

    int msgCount;
    public int triggerCount;
    public Element[] waiting;
    List<Element> tempList;
    public void BeforeBuildDependence() {
        triggerCount = 0;
        waiting = null;
        tempList = new List<Element>();
    }
    public void AddWaiting(Element waiting) {
        tempList.Add(waiting);
    }
    public void AddTriggerCount()
    {
        triggerCount++;
    }
    public void AfterBuildDependence()
    {
        if (tempList.Count > 0)
        {
            waiting = tempList.ToArray();
            tempList = null;
        }
    }

    public void BeforeShuffle()
    {
        msgCount = 0;
        if (triggerCount > 0)
            state = ElementState.ShuffleWaitTriggerMsg;
        else
            state = ElementState.ShuffleCanUse;
    }
}
