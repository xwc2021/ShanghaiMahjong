using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShuffleState { NoPick,Picking,PickFinish}
public class Group : MonoBehaviour {

    public ShuffleState state;
    public bool hasDependence = false;

    public void PickOne()
    {
        //如果elment裡有outputTrigger就發送
    }
}
