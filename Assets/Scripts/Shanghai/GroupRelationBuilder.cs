using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEditor;
using System;

//用來記錄相依性
[System.Serializable]
public class GroupRelation
{
    [SerializeField]
    Group trigger;//先(前置條件)
    public Group GetTrigger() { return trigger; }

    [SerializeField]
    Group waiting;//後
    public Group GetWaiting() { return waiting; }

    public GroupRelation(Group trigger, Group waiting)
    {
        this.trigger = trigger;
        this.waiting = waiting;
    }

    public void ReverseGroupRealtion()
    {
        var temp = waiting;
        waiting = trigger;
        trigger = temp;
    }

    public bool IsRightSideLink() {
        var triggerX = trigger.GetPosX(); var waitingX = waiting.GetPosX();
        return triggerX < waitingX;
    }

    public ElementRelation GetElementRelation() {
        if (IsRightSideLink())
            return new ElementRelation(
                    trigger.GetTailElement(),
                    waiting.GetHeadElement());
        else
            return new ElementRelation(
                    trigger.GetHeadElement(),
                    waiting.GetTailElement());
    }

    public Element GetWaitingElement()
    {
        if (IsRightSideLink())
            return waiting.GetHeadElement();
        else
            return waiting.GetTailElement();
    }

    public Element GetTriggerElement()
    {
        if (IsRightSideLink())
            return trigger.GetTailElement();
        else 
            return trigger.GetHeadElement();
    }
}

//用來記錄相依性
[System.Serializable]
public class ElementRelation
{
    [SerializeField]
    Element trigger;//先(前置條件)
    public Element GetTrigger() { return trigger; }

    [SerializeField]
    Element waiting;//後
    public Element GetWaiting() { return waiting; }

    public ElementRelation(Element trigger, Element waiting)
    {
        this.trigger = trigger;
        this.waiting = waiting;
    }

    public void InjectDependence()
    {
        var trigger = GetTrigger();
        var waiting = GetWaiting();
        trigger.AddWaiting(waiting);
        waiting.AddTriggerCount();
    }
}

[System.Serializable]
public class MultiSegmentList<T>
{
    static int NotInArray = -1;
    static List<T> Nothing = new List<T>();

    public MultiSegmentList(int count)
    {
        list = new List<T>();
        SegmentBeginIndex = new int[count];
        SegmentCount = new int[count];
    }

    [SerializeField]
    List<T> list;

    [SerializeField]
    int[] SegmentBeginIndex;
    public void SetGroupLinksBeginIndex(int index, int value) { SegmentBeginIndex[index] = value; }

    [SerializeField]
    int[] SegmentCount;
    public void SetGroupLinksCount(int index, int value) { SegmentCount[index] = value; }

    public List<T> GetList() { return list; }

    public List<T> GetSegment(int index)
    {
        if (SegmentBeginIndex[index] == NotInArray)
            return Nothing;

        return list.GetRange(SegmentBeginIndex[index], SegmentCount[index]);
    }

    public void Add(T item) {
        list.Add(item);
    }

    int oldCount;
    public void BeginRecord()
    {
        oldCount = list.Count;
    }

    public void EndRecord(int index)
    {
        var newCount = list.Count;
        if (oldCount != newCount)
        {
            SegmentBeginIndex[index] = oldCount;
            SegmentCount[index] = newCount - oldCount;
        }
        else
        {
            SegmentBeginIndex[index] = NotInArray;
            SegmentCount[index] = 0;
        }
    }
}

//如何讓Unity的Inspector可以接受泛型序列化
//https://forum.unity.com/threads/how-do-you-get-a-generic-template-class-to-show-in-the-inspector.341367/
[System.Serializable]
public class GroupRelationMSList : MultiSegmentList<GroupRelation>
{
    public GroupRelationMSList(int count):base(count){
    }
}

[System.Serializable]
public class GroupMSList : MultiSegmentList<Group>
{
    public GroupMSList(int count) : base(count)
    {
    }
}

[System.Serializable]
public class RelationManager {

    public void BeforeBuild(int floorCount) {
        downToUpLinks = new List<ElementRelation>();
        groupLinks = new GroupRelationMSList(floorCount);
    }

    [SerializeField]
    List<ElementRelation> downToUpLinks;//上下層之間的ElementRelation
    public List<ElementRelation> GetDownToUpLinks() { return downToUpLinks; }

    public void AddDownToUpLink(Element trigger, Element waiting) {
        downToUpLinks.Add(new ElementRelation(trigger, waiting));
    }

    [SerializeField]
    GroupRelationMSList groupLinks;//同1層Floor的GroupRelation
    public void GroupLinksBeginRecord(){groupLinks.BeginRecord();}
    public void GroupLinksEndRecord(int floor) {groupLinks.EndRecord(floor);}

    public List<GroupRelation> GetTotalGroupLinks()
    {
        return groupLinks.GetList();
    }
    public List<GroupRelation> GetGroupLinks(int floor)
    {
        return groupLinks.GetSegment(floor);
    }


    public void AddGrouppLink(Group trigger, Group waiting)
    {
        groupLinks.Add(new GroupRelation(trigger, waiting));
    }

    Dictionary<Group, List<GroupRelation>> outputArrows;//key是trigger
    Dictionary<Group, List<GroupRelation>> inputArrows;//key是waiting
    public bool HasGroupRelation(Group group) { return inputArrows.ContainsKey(group); }
    public List<GroupRelation> GetGroupRelation(Group group) { return inputArrows[group]; }

    void FillDictionary(Group key,GroupRelation value, Dictionary<Group, List<GroupRelation>> arrows)
    {
        if (!arrows.ContainsKey(key))
        {
            var list = new List<GroupRelation>();
            list.Add(value);
            arrows.Add(key, list);
        }
        else
        {
            var list = arrows[key];
            list.Add(value);
        }
    }

    public void BuildGroupDependenceSearchHelper()
    {
        outputArrows = new Dictionary<Group, List<GroupRelation>>();
        inputArrows = new Dictionary<Group, List<GroupRelation>>();

        foreach(var relation in groupLinks.GetList()) {
            var trigger =relation.GetTrigger();
            var waiting = relation.GetWaiting();
            FillDictionary(trigger, relation, outputArrows);
            FillDictionary(waiting, relation, inputArrows);
        }
       
        Debug.Log("BuildGroupDependenceSearchHelper");
    }

    public void ReverseInputArrow(Group group) {
        BuildGroupDependenceSearchHelper();
        if (!inputArrows.ContainsKey(group))
        {
            Debug.Log("Can not find input arrow");
            return;
        }
        var links = inputArrows[group];

        Debug.Log(links.Count);
        //轉向
        foreach (var link in links)
            link.ReverseGroupRealtion();
    }

    public void ReverseInputArrows(Group group)
    {
        BuildGroupDependenceSearchHelper();
        if (!inputArrows.ContainsKey(group))
        {
            Debug.Log("Can not find input arrow");
            return;
        }
        var links = inputArrows[group];

        var findingSet = new HashSet<GroupRelation>();
        var queue = new List<GroupRelation>();
        queue.AddRange(links.ToArray());
        while (queue.Count > 0) {
            //取出head放入findingList
            var head = queue[0];
            queue.Remove(head);

            if(!findingSet.Contains(head))
                findingSet.Add(head);
            //再把head的所有links放入queue
            if (inputArrows.ContainsKey(head.GetTrigger()))
            {
                var hisLinks = inputArrows[head.GetTrigger()];
                queue.AddRange(hisLinks);
            }
                
        }

        Debug.Log(findingSet.Count);
        //轉向
        foreach (var link in findingSet)
            link.ReverseGroupRealtion();
    }
}

public class GroupRelationBuilder : MonoBehaviour {
    [SerializeField]
    Transform groupsContainer;
    [SerializeField]
    Transform elementsContainer;
    [SerializeField]
    Group groupPrefab;
    [SerializeField]
    Element elementPrefab;

    [SerializeField]
    VoxelBuilder voxelBuilder;
    public VoxelBuilder GetVoxelBuilder() { return voxelBuilder; }

    [SerializeField]
    Game game;

    public void AddToShufflingSet(Group group)
    {
        game.AddToShufflingList(group);
    }

    [SerializeField]
    GroupMSList groups;

    public List<Group> GetTotalGroupList()
    {
        return groups.GetList();
    }

    public List<Group>  GetGroupList() {
        return GetGroupList(GetNotFloorIndex());
    }

    int GetNotFloorIndex() { return voxelBuilder.GetNowFloorIndex(); }

    public List<Group> GetGroupList(int floor)
    {
        return groups.GetSegment(floor);
    }

    void BeforeBuildGroup() {
        Tool.Clear(groupsContainer);
        Tool.Clear(elementsContainer);
        groups = new GroupMSList(voxelBuilder.GetFloor());
    }

    void BeforeBuildLink()
    {
        relationManager.BeforeBuild(voxelBuilder.GetFloor());
    }

    public void BuildGroups() {
        //(1)建立Group(每1層由左下角開始水平掃描)
        BeforeBuildGroup();
        for (var f = 0; f < voxelBuilder.GetFloor(); ++f)
        {
            BuildGroupInTheFloor(f);
        }
    }

    public void BuildLinks()
    {
        BeforeBuildLink();
        //(2)每1層作Link(Relation)
        for (var f = 0; f < voxelBuilder.GetFloor(); ++f)
            MakeLinksInTheFloor(f);

        //(3)上下層作Link(Relation)
        for (var f = voxelBuilder.GetFloor() - 1; f >= 1; --f)
            MakeLinkBetween2Floor(f, f - 1);
    }

    public void ReverseInputArrows()
    {
        relationManager.ReverseInputArrows(nowSelectGroup);
    }

    public void ReverseInputArrow()
    {
        relationManager.ReverseInputArrow(nowSelectGroup);
    }

    public void BuildElementDependence()
    {
        BeforeBuildElementDependence();
        InjectElementDependence();
        AfterBuildElementDependence();

        Debug.Log("BuildElementDependence");
    }

    public List<GroupRelation> GetGroupRelation(Group group)
    {
        return relationManager.GetGroupRelation(group);
    }

    public void BuildForGame()
    {
        relationManager.BuildGroupDependenceSearchHelper();//這個無法記在Unity裡
        BuildElementDependence();
        foreach (var g in groups.GetList())
        {
            g.CheckIfHasFloorLink();
            g.hasGroupRelation =relationManager.HasGroupRelation(g);
        }
    }

    public void BeforeShuffle()
    {
        foreach (var g in groups.GetList()) {
            g.SetIsFirstShuffle(true);
            g.BeforeShuffle();
        }
        var elementList = GetElementsFromGroups(groups.GetList());
        foreach (var e in elementList)
            e.BeforeShuffle();
    }

    public void PickIndependentGroup()
    {
        foreach (var g in groups.GetList())
        {
            if (g.hasGroupRelation)
                continue;
            if (g.hasFloorLink)
                continue;

            game.AddToShufflingList(g);
        }
    }

    void BeforeBuildElementDependence()
    {
        var elementList = GetElementsFromGroups(groups.GetList());
        foreach (var e in elementList)
            e.BeforeBuildDependence();
    }

    void AfterBuildElementDependence()
    {
        var elementList = GetElementsFromGroups(groups.GetList());
        foreach (var e in elementList)
            e.AfterBuildDependence();
    }

    public void InjectElementDependence()
    {
        //(4)為Element寫入triggerCount和waitings
        var downToUpLinks = relationManager.GetDownToUpLinks();
        foreach (var elementRelation in downToUpLinks)
            elementRelation.InjectDependence();

        var groupLinks = relationManager.GetTotalGroupLinks();
        foreach (var groupRelation in groupLinks) {
            var elementRelation=groupRelation.GetElementRelation();
            elementRelation.InjectDependence();
        }
    }

    void MakeLinksInTheFloor(int floor) {
        relationManager.GroupLinksBeginRecord();
        var groups =GetGroupList(floor);
        foreach (var g in groups) {
            var groupTail = g.GetTailElement();
            //https://plus.google.com/u/0/+XiangweiChiou/posts/aCJpgroisHx
            //groupTail往右測式2個voxel
            var y = groupTail.y; var x = groupTail.x;
            var voxels = new Voxel[] {
                GetVoxel(floor, y+1, x+2),
                GetVoxel(floor, y-1, x+2)
            };
            foreach (var v in voxels)
            {
                if (v != null && v.IsUse())
                    relationManager.AddGrouppLink(v.group,g );
            }
        }
        relationManager.GroupLinksEndRecord(floor);
    }

    List<Element> GetElementsFromGroups(List<Group> groups) {
        var elementList = new List<Element>();
        foreach (var g in groups)
            elementList.AddRange(g.GetElements());
        return elementList;
    }

    void MakeLinkBetween2Floor(int floor, int lowerFloor) {
        var groups = GetGroupList(floor);
        var elementList = GetElementsFromGroups(groups);
        foreach (var e in elementList)
            MakeLinkToLowerFloor(e);
    }

    void MakeLinkToLowerFloor(Element element) {
        //https://plus.google.com/u/0/+XiangweiChiou/posts/aCJpgroisHx
        //往下層測式9個voxel
        var lowerFloor = element.floor-1; var y = element.y; var x = element.x;
        var voxels = new Voxel[] {
            GetVoxel(lowerFloor, y+1, x-1),
            GetVoxel(lowerFloor, y, x-1),
            GetVoxel(lowerFloor, y-1, x-1),
            GetVoxel(lowerFloor, y+1, x),
            GetVoxel(lowerFloor, y, x),
            GetVoxel(lowerFloor, y-1, x),
            GetVoxel(lowerFloor, y+1, x+1),
            GetVoxel(lowerFloor, y, x+1),
            GetVoxel(lowerFloor, y-1, x+1)
        };
        foreach (var v in voxels) {
            if (v != null && v.IsUse())
                relationManager.AddDownToUpLink(v.element, element);
        }  
    }

    Voxel GetVoxel(int floor,int y,int x){
        return voxelBuilder.GetVoxel(floor, y, x);
    }

    void BuildGroupInTheFloor(int floor)
    {
        groups.BeginRecord();
        for (var y = 0; y < voxelBuilder.CountY(); ++y)
        {
            var lines = GetLines(floor, y);
            for (var i = 0; i < lines.Count; ++i) {//1個line會對映到1個group
                var group = CreateGroup();
                var voxels = lines[i];
                var elementList = new List<Element>();
                for (var k = 0; k < voxels.Length; ++k) {
                    var element = CreateElement();
                    var voxel = voxels[k];
                    element.Set(voxel);
                    elementList.Add(element);

                    //為了之後建立relation方便
                    voxel.group = group;
                    voxel.element = element;
                }
                var voxelBegin = voxels[0];
                var voxelEnd = voxels[voxels.Length - 1];
                group.Setpos(voxelBegin);
                group.Set(voxelBegin.floor, voxelBegin.y, voxelBegin.x, voxelEnd.x);
                group.AddElements(elementList.ToArray());
                groups.Add(group);
            }
        }
        groups.EndRecord(floor);
    }

    Group CreateGroup()
    {
        return Instantiate<Group>(groupPrefab,groupsContainer);
    }

    Element CreateElement()
    {
        return Instantiate<Element>(elementPrefab, elementsContainer);
    }

    List<Voxel[]> GetLines(int floor, int y)
    {
        List<Voxel[]> batch = new List<Voxel[]>();
        List<Voxel> line=new List<Voxel>();
        bool adding=false;

        var x = 0;
        while(x < voxelBuilder.CountX())
        {
            var voxel = GetVoxel(floor, y, x);
            if (voxel.IsUse())
            {
                adding = true;
                line.Add(voxel);
                x = x + 2;//相連會差2格
                continue;
            }
            else
            {
                if (adding) {
                    batch.Add(line.ToArray());
                    line.Clear();
                    adding = false;
                } 
            }
            x = x  +1;
        }

        //如果最後1次add是在末端
        if(adding)
            batch.Add(line.ToArray());

        return batch;
    }

    public void SetNowFloorIndex(int offset)
    {
        voxelBuilder.SetNowFloorIndex(offset);
    }

    public RelationManager relationManager;

    public List<GroupRelation> GetGroupLinks()
    {
        return relationManager.GetGroupLinks(GetNotFloorIndex());
    }

    public List<GroupRelation> GetTotalGroupLinks()
    {
        return relationManager.GetTotalGroupLinks();
    }

    public List<ElementRelation> GetDownToUpLinks()
    {
        return relationManager.GetDownToUpLinks();
    }

    public bool showTotalFloor=false;

    [SerializeField]
     Group nowSelectGroup;
    public Group GetNowSelectGroup() { return nowSelectGroup; }
    public void SetNowSelectGroup(Group group) { nowSelectGroup=group; }
    public void ClearNowSelectGroup() { nowSelectGroup=null; }
}