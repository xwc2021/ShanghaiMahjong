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


    Group GetRandomGroupInSufflingList(){ return GetRandomGroupInList(shufflingList); }

    Group GetRandomGroupInList(List<Group> list)
    {
        int index = Random.Range(0, list.Count);
        return list[index];
    }

    //取得shufflingList裡有outputArrow的group，再用深度排序後，傳回最最前段班
    List<Group> OutputArrowOrderByDepthFilter(List<Group> source)
    {
        var list = new List<Group>();
        foreach (var g in source)
            if (g.hasOutputArrow)
                list.Add(g);

        if (list.Count == 0)
            return source;

        //深度由小到大
        list.Sort((a, b) => {
            if (a.depth < b.depth)
                return -1;
            else
                return 1;
        });

        //取出最前面的那一段
        return GetFirstSegment(list, list[0].depth, GetGroupDepth);
    }

    delegate int GetGroupField(Group group);
    int GetGroupFloor(Group group) { return group.floor; }
    int GetGroupDepth(Group group) { return group.depth; }
    int GetGroupShuffeNotUseCount(Group group) { return group.GetShuffeNotUseCount(); }

    List<Group> GetFirstSegment(List<Group> list,int value, GetGroupField funptr)
    {
        var Count = 1;
        for (var i = 1; i < list.Count; ++i)
        {
            if (funptr(list[i]) == value)
                ++Count;
            else
                break;
        }
        return list.GetRange(0, Count);
    }

    List<Group> OrderByFloorFilter(List<Group> source)
    {
        var list = new List<Group>(source.ToArray());
        //由小排到大
        list.Sort((a, b) =>
        {
            if (a.floor < b.floor)
                return -1;
            else
                return 1;
        });

        //取出最前面的那一段
        return GetFirstSegment(list, list[0].floor, GetGroupFloor);
    }

    List<Group> OrderByNotUseCountFilter(List<Group> source)
    {
        var list = new List<Group>(source.ToArray());
        //由大排到小
        list.Sort((a, b) =>
        {
            if (a.GetShuffeNotUseCount() < b.GetShuffeNotUseCount())
                return 1;
            else
                return -1;
        });

        //取出最前面的那一段
        return GetFirstSegment(list, list[0].GetShuffeNotUseCount(), GetGroupShuffeNotUseCount);
    }

    Group GetRandomGroupInSufflingListWithConstraint()
    {
        var list = shufflingList;


        //有OutputArrow優先
        list = OutputArrowOrderByDepthFilter(list);

        //空位置多的優先
        list = OrderByNotUseCountFilter(list);

        return GetRandomGroupInList(list);
    }

    //(3)如何從Group裡挑中Element
    Element PickElementInGroup(Group group)
    {
        var element = group.PickElementInGroup();
        return element;
    }

    void AfterPickElement(Group group)
    {
        //如果group滿了，設定state=ShuffleFinish，並從ShufflingList中移出
        if (group.IsSuffleFinish())
            RemoveFromShufflingList(group);
        else if (!group.CanSetElement())
            RemoveFromShufflingList(group);
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

    bool doShuffling;
    //洗牌
    public bool Shuffle()
    {
        doShuffling = true;
        while (doShuffling && shufflingList.Count>0)
        {
            ShuffleOneStep();
        }

        return shufflingList.Count == 0 && doShuffling;
    }

    Element PickE2(Element e1)
    {
        int i = 0;
        while (true)
        {
            ++i;
            if (i > 20)
            {
                //這種case
                //https://photos.google.com/share/AF1QipOBIcPnUrycdqIu3uWtm2fF2xS9CTYLqKd62yZG89l_9G5ShEIrZdYCAumpJTCkOQ/photo/AF1QipN9U3yuo-xD9aemcdLvTsj7rza8-csfurpH5qgt?key=UEVQZEpLT3NLMjhXRklQNUp3N1Q5dHM0QXVNd3pB
                Debug.Log("出不去");
                return null;
            }
                
            var g2 = GetRandomGroupInSufflingList();
            g2.MemoryState();
            var e2 = PickElementInGroup(g2);
            if (e2.group == e1.group && g2.shuffeUseCount>=3 && e2.IsNeighbor(e1))
            {
                //Debug.Log("RollBack "+ g2.name);
                g2.RollBack();
                continue;
            }
            AfterPickElement(g2);
            return e2;
        }
    }

    public void ShuffleOneStep() {

        //(2)從ShufflingList裡隨機挑出2個group
        //為了避免這種case
        //https://photos.google.com/share/AF1QipOBIcPnUrycdqIu3uWtm2fF2xS9CTYLqKd62yZG89l_9G5ShEIrZdYCAumpJTCkOQ/photo/AF1QipM49GnYdlrx7vOzpi68JuSV3NKFSVh6OwjfELcq?key=UEVQZEpLT3NLMjhXRklQNUp3N1Q5dHM0QXVNd3pB
        var g1 = GetRandomGroupInSufflingListWithConstraint();
        var e1 = PickElementInGroup(g1);
        AfterPickElement(g1);

        //有可能出現g2不存在
        //https://photos.google.com/share/AF1QipOBIcPnUrycdqIu3uWtm2fF2xS9CTYLqKd62yZG89l_9G5ShEIrZdYCAumpJTCkOQ/photo/AF1QipOL8-TRsdE-XKHVYC7brqQYyzclaqxPc6NZ5XIQ?key=UEVQZEpLT3NLMjhXRklQNUp3N1Q5dHM0QXVNd3pB
        if (shufflingList.Count == 0)
        {
            Debug.Log("洗牌失敗");
            doShuffling = false;
            return;
        }

        var e2 = PickE2(e1);
        if (e2 == null)
        {
            Debug.Log("洗牌失敗");
            doShuffling = false;
            return;
        }
            
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
        Tool.Clear(this.transform);
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

        if (!DebugSuffle)
        {
            while (true)
            {
                Debug.Log("開始洗牌");
                BeforeShuffle();
                var ok = Shuffle();
                if (ok)
                {
                    Debug.Log("Shuffle Finish");
                    break;
                }
            }
        }
        else
        {
            Debug.Log("開始洗牌");
            BeforeShuffle();
        }
            
    }

    void Start()
    {
        groupRelationBuilder.BuildForGame();
        BuildNewGame();
    }
}
