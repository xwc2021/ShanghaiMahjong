using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

    [SerializeField]
    GroupRelationBuilder groupRelationBuilder;

    public List<Group> shufflingList;

    List<Group> playingList;

    List<Element[]> pairs;

    Group GetRandomGroupInSufflingList()
    {
        int index = Random.Range(0, shufflingList.Count);
        return shufflingList[index];
    }

    //(3)如何從Group裡挑中Element
    Element PickElementInGroup(Group group)
    {
        //還沒實作
        return null;
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

        //如果group滿了，設定state=ShuffleFinish，並從ShufflingList中移出
        if (group.IsSuffleFinish())
        {
            group.state = GroupState.GameFinish;
            RemoveFromShufflingList(group);
        }
        else if (!group.CanSetElement())
            RemoveFromShufflingList(group);
    }

    //洗牌
    public void Shuffle()
    {
        //(1)挑出沒有相依性的Group，放入Game的ShufflingList
        groupRelationBuilder.PickIndependentGroup();

        //while (shufflingList.Count>0)
        {
            //(2)從ShufflingList裡隨機挑出2個group
            var g1 =GetRandomGroupInSufflingList();
            var e1=PickElementInGroup(g1);

            var g2 = GetRandomGroupInSufflingList();
            var e2 = PickElementInGroup(g2);

            pairs.Add(new Element[] { e1, e2 });
        }
    }

    public void AddToShufflingList(Group group)
    {
       shufflingList.Add(group);
       group.isInSuffleList = true;
    }

    public void RemoveFromShufflingList(Group group)
    {
        shufflingList.Remove(group);
        group.isInSuffleList = false;
    }

    void BeforeShuffle()
    {
        pairs = new List<Element[]>();
        shufflingList = new List<Group>();
        groupRelationBuilder.BeforeShuffle();
    }

    //開始新的一局
    void BuildNewGame()
    { 
        BeforeShuffle();
        Shuffle();
    }

    void Awake()
    {
        groupRelationBuilder.BuildForGame();
        BuildNewGame();
    }
}
