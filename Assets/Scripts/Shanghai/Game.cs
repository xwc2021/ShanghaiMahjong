using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

    [SerializeField]
    GroupRelationBuilder groupRelationBuilder;

    public List<Group> groupList;

    
    public List<Group> ShufflingList;

    List<Group> playingList;

    Group GetRandomGroupInSufflingList()
    {
        int index = Random.Range(0, ShufflingList.Count);
        return ShufflingList[index];
    }

    void ShuffleGroup(Group group)
    {     
        if (group.hasGroupRelation)//如果是有同層相依性的group
        {
            //ShuffleNotUsing->挑中端點
            //ShuffleUsing->往左/右挑1個
        }
        else
        {
            //如果是只有上下層相依性的group
            if (group.hasFloorLink)
            {
                //ShuffleNotUsing->從ready的elments中随機挑出1個element
                //ShuffleUsing->nowPickIndex左/右挑1個element(要ready的才行)
            }
            else//沒有相依性的group
            {
                //ShuffleNotUsing->随機挑出1個element
                //ShuffleUsing->nowPickIndex左/右挑1個element
            }
        }
    }

    //洗牌
    public void Shuffle()
    {
        //(1)挑出沒有相依性的Group，放入Game的ShufflingList
        groupRelationBuilder.PickIndependentGroup();

        //while (ShufflingList.Count>0)
        if(false)
        {
            //(2)從ShufflingList裡隨機挑出1個group
            var group =GetRandomGroupInSufflingList();
            ShuffleGroup(group);

            //如果group滿了，設定state=ShuffleFinish，並從ShufflingList中移出
            if (group.IsSuffleFinish()) {
                group.state = GroupState.GameFinish;
                ShufflingList.Remove(group);
            }
        }
    }

    public void AddToShufflingSet(Group group)
    {
       ShufflingList.Add(group);
    }

    void BeforeShuffle()
    {
        ShufflingList = new List<Group>();
        groupRelationBuilder.BeforeShuffle();
    }

    //開始新的一局
    void BuildNewGame()
    { 
        BeforeShuffle();
        Shuffle();
    }

    private void Awake()
    {
        groupRelationBuilder.BuildForGame();
        BuildNewGame();
    }
}
