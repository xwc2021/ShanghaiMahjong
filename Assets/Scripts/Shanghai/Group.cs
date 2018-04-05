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

    public void PickOne()
    {
        //如果elment裡有outputTrigger就發送
    }
}
