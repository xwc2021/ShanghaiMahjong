﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

    public bool DebugSuffle=false;

    [SerializeField]
    GameObject voxelVixibleOdd;
    [SerializeField]
    GameObject voxelVisibleEven;

    [SerializeField]
    GroupRelationBuilder groupRelationBuilder;

    HashSet<Group> shufflingSet;
    public List<Group> shufflingList;
    public List<Element> pairsOne;
    public List<Element> pairsTwo;
    List<Group> playingList;

    Group GetRandomGroupInSufflingList()
    {
        int index = Random.Range(0, shufflingList.Count);
        return shufflingList[index];
    }

    //(3)如何從Group裡挑中Element
    Element PickElementInGroup(Group group)
    {
        var element = group.PickElementInGroup();

        //如果group滿了，設定state=ShuffleFinish，並從ShufflingList中移出
        if (group.IsSuffleFinish())
            RemoveFromShufflingList(group);
        else if (!group.CanSetElement())
            RemoveFromShufflingList(group);

        return element;
    }

    [SerializeField]
    PrefabHolder prefabHolder;

    GameObject GetMahjong() { return prefabHolder.GetRandomPrefab(); }
    GameObject GetVoxelVixible() {
        var isEven = pairsOne.Count % 2 == 0;
        return isEven ? voxelVisibleEven : voxelVixibleOdd;
    }

    void AddPair(Element e1,Element e2)
    {
        pairsOne.Add(e1);
        pairsTwo.Add(e2);

        var obj = GetMahjong();

        var v1 = Instantiate<GameObject>(obj, this.transform);
        var v2 = Instantiate<GameObject>(obj, this.transform);

        v1.transform.localPosition = e1.transform.localPosition;
        v2.transform.localPosition = e2.transform.localPosition;

        v1.name = e1.name;
        v2.name = e2.name;
    }

    //洗牌
    public void Shuffle()
    {
        while (shufflingList.Count>0)
        {
            ShuffleOneStep();
        }

        Debug.Log("Shuffle Finish");
    }

    public void ShuffleOneStep() {

        //(2)從ShufflingList裡隨機挑出2個group
        var g1 = GetRandomGroupInSufflingList();
        var e1 = PickElementInGroup(g1);

        var g2 = GetRandomGroupInSufflingList();
        var e2 = PickElementInGroup(g2);

        AddPair(e1, e2);
    }

    public void AddToShufflingSet(Group group)
    {
       if (shufflingSet.Contains(group))
            return;

       shufflingSet.Add(group);
       shufflingList.Add(group);
       group.isInSuffleList = true;
    }

    public void RemoveFromShufflingList(Group group)
    {
        shufflingSet.Remove(group);
        shufflingList.Remove(group);
        group.isInSuffleList = false;
    }

    void BeforeShuffle()
    {
        pairsOne = new List<Element>();
        pairsTwo = new List<Element>();
        shufflingSet = new HashSet<Group>();
        shufflingList = new List<Group>();
        groupRelationBuilder.BeforeShuffle();

        //(1)挑出沒有相依性的Group，放入Game的ShufflingList
        groupRelationBuilder.PickIndependentGroup();
    }

    //開始新的一局
    void BuildNewGame()
    {
        if (groupRelationBuilder.totalElementCount % 2 != 0)
        {
            Debug.Log("洗牌機器人：不是偶數喔!不幫你洗");
            return;
        }

        BeforeShuffle();
        if(!DebugSuffle)
            Shuffle();
    }

    void Start()
    {
        groupRelationBuilder.BuildForGame();
        BuildNewGame();
    }
}
