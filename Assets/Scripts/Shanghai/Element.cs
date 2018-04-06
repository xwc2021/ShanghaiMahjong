using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//前3個是洗牌狀態，最後1個是玩家刪除
public enum ElementState {
    ShuffleWaitAllTrigger,//等待所有trigger都滿足才能被配置
    ShuffleReady,//可以被配置
    ShuffleSet,//已經被配置
    Delete//玩家已經消除
}
public class Element : MonoBehaviour {

    public ElementState state;
    public int floor;
    public int y;
    public int x;

    public int triggerCount;
    public Element[] waiting;

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
        //如果所有前置條件達成
        //把自己所在的Group加進Game的groupList
    }
}
