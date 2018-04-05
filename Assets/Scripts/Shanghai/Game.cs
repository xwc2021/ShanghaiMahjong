using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

    public List<Group> groupList;

    List<Group> ShufflingList;

    //洗牌
    public void Shuffle()
    {
        //(1)挑出沒有相依性的Group，放入Game的ShufflingList
        //(2)從ShufflingList裡隨機挑出1個group
        while (false)
        {
            //如果不是
            {
                //ShuffleNotUsing->随機挑出1個element
                //ShuffleUsing->nowPickIndex左/右挑1個element
            }
            //如果是有上下層相依性的group
            {
                //ShuffleNotUsing->從ready的elments中随機挑出1個element
                //ShuffleUsing->nowPickIndex左/右挑1個element(要ready的才行)
            }
            //如果是有同層相依性的group
            {
                //ShuffleNotUsing->挑中端點
                //ShuffleUsing->往左/右挑1個
            }

            //如果group滿了，設定state=ShuffleFinish，並從groupList中移出
        }

    }
}
