using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupRelationBuilder : MonoBehaviour {

    [SerializeField]
    VoxelBuilder mahjongBuilder;

    [SerializeField]
    Game game;

    [SerializeField]
    List<Group> groupList;

    public void Build()
    {
        //(1)建立Group(每1層由左上角開始水平掃描)
        for (var f = 0; f < mahjongBuilder.GetFloor(); ++f) {
        }

        //(2)每1層作Link(Relation)
        //(3)上下層作Link(Relation)
        //(4)為Element綁定OutputTrigger和InputReceiver
    }

    Voxel GetNode(int floor,int y,int x){
        return mahjongBuilder.GetNode(floor, y, x);
    }

    void BuildGroupInTheFloor(int floor)
    {
        for (var y = 0; y < mahjongBuilder.GetY(); ++y)
        {
            for (var x = 0; x < mahjongBuilder.GetX(); ++x)
            {
            }
        }
    }

    //洗牌
    public void Shuffle()
    {
        //(1)挑出沒有相依性的Group，放入Game的ShufflingList
        //(2)從ShufflingList裡隨機挑出1個group
        while (false)
        {
            //如果不是
            if (false)
            {
                //ShuffleNotUsing->随機挑出1個element
                //ShuffleUsing->nowPickIndex左/右挑1個element
            }
            //如果是有相依性的group
            if (true)
            {
                //ShuffleNotUsing->從ready的elments中随機挑出1個element
                //ShuffleUsing->nowPickIndex左/右挑1個element(要ready的才行)
            }


            //如果group滿了，設定state=ShuffleFinish，並從groupList中移出
        }

    }

}
