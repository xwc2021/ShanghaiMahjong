using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//前3個是洗牌狀態，最後1個是玩家刪除
public enum ElementState { ShuffleWaiting, ShuffleReady, ShufflePick, Delete }
public class Element : MonoBehaviour {

    public ElementState state;
}
