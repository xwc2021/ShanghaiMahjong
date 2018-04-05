using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupRelationBuilder : MonoBehaviour {
    public MahjongBuilder mahjongBuilder;

    public void Build()
    {
        //(1)建立Group(每1層由左上角開始水平掃描)
        //(2)每1層作Link(Relation)
        //(3)上下層作Link(Relation)
        //(4)為Element綁定OutputTrigger和InputReceiver
    }

    //洗牌
    public void Shuffle()
    {
        //(1)挑出沒有相依性的Group，放入Game的groupList
        //(2)從groupList裡隨機挑出1個group
        while (false)
        {
            //如果不是
            if (false)
            {
                //NoPick->随機挑出1個element
                //Picking->nowPickIndex左/右挑1個element
            }
            //如果是有相依性的group
            if (true)
            {
                //NoPick->從ready的elments中随機挑出1個element
                //Picking->nowPickIndex左/右挑1個element(要ready的才行)
            }


            //如果group滿了，設定state=PickFinish，並從groupList中移出
        }

    }

}
