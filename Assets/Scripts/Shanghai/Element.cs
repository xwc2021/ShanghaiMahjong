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

    OutputTrigger[] outputs;
    InputReceiver input;

    public void BeforeSuffle() {
        outputs = GetComponents<OutputTrigger>();
        input = GetComponent<InputReceiver>();
    }
}
