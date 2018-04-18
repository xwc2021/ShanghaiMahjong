using System.Collections;
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

    Group GetRandomGroupInSufflingListHasOutputArrowFirst()
    {
        var outputArrowList = new List<Group>();
        foreach (var g in shufflingList)
            if (g.hasOutputArrow)
                outputArrowList.Add(g);

        if (outputArrowList.Count == 0)
            return GetRandomGroupInSufflingList();

        //由小到大
        outputArrowList.Sort((a, b) => {
            if (a.depth < b.depth)
                return -1;
            else
                return 1;
        });

        var Count = 1;
        var depth = outputArrowList[0].depth;
        for (var i = 1; i < outputArrowList.Count; ++i)
        {
            if (outputArrowList[i].depth == depth)
                ++Count;
            else
                break;
        }

        int index = Random.Range(0, Count);
        var nowGroup = outputArrowList[index];
        return nowGroup;
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
        //為了避免這種case
        //https://photos.google.com/share/AF1QipOBIcPnUrycdqIu3uWtm2fF2xS9CTYLqKd62yZG89l_9G5ShEIrZdYCAumpJTCkOQ/photo/AF1QipM49GnYdlrx7vOzpi68JuSV3NKFSVh6OwjfELcq?key=UEVQZEpLT3NLMjhXRklQNUp3N1Q5dHM0QXVNd3pB
        var g1 = GetRandomGroupInSufflingListHasOutputArrowFirst();
        var e1 = PickElementInGroup(g1);

        var g2 = GetRandomGroupInSufflingList();
        var e2 = PickElementInGroup(g2);

        //為了避免這種case
        //https://photos.google.com/share/AF1QipOBIcPnUrycdqIu3uWtm2fF2xS9CTYLqKd62yZG89l_9G5ShEIrZdYCAumpJTCkOQ/photo/AF1QipMV8fgMmA9pVUzs1-GMLiPq8DooJLJv9IUhyUxY?key=UEVQZEpLT3NLMjhXRklQNUp3N1Q5dHM0QXVNd3pB
        //所以延後到這時才通知等待中的牌
        e1.SendMsg();
        e2.SendMsg();

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
