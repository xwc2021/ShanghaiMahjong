using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEditor;

//用來記錄相依性
[System.Serializable]
public class GroupRelation {

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

    public GroupRelation GetReverseGroupRealtion()
    {
        return new GroupRelation(waiting, trigger);
    }

    public ElementRelation GetElementRelation() {
        var triggerX = trigger.GetPosX(); var waitingX = waiting.GetPosX();
        if (triggerX < waitingX)//trigger在左邊
            return new ElementRelation(
                    trigger.GetTailElement(),
                    waiting.GetHeadElement());
        else //trigger在右邊
            return new ElementRelation(
                    trigger.GetHeadElement(),
                    waiting.GetTailElement());
    }

    public Element GetWaitingElement()
    {
        var triggerX = trigger.GetPosX(); var waitingX = waiting.GetPosX();
        if (triggerX < waitingX)//trigger在左邊
            return waiting.GetHeadElement();
        else //trigger在右邊
            return waiting.GetTailElement();
    }

    public Element GetTriggerElement()
    {
        var triggerX = trigger.GetPosX(); var waitingX = waiting.GetPosX();
        if (triggerX < waitingX)//trigger在左邊
            return trigger.GetTailElement();
        else //trigger在右邊
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
}

[System.Serializable]
public class RelationManager {

    public void BeforeBuild() {
        downToUpLinks = new List<ElementRelation>();
        groupLinks = new List<GroupRelation>();
    }

    [SerializeField]
    List<ElementRelation> downToUpLinks;//上下層之間的ElementRelation

    public void AddDownToUpLink(Element trigger, Element waiting) {
        downToUpLinks.Add(new ElementRelation(trigger, waiting));
    }

    [SerializeField]
    List<GroupRelation> groupLinks;//同1層Floor的GroupRelation

    public void AddGrouppLink(Group trigger, Group waiting)
    {
        groupLinks.Add(new GroupRelation(trigger, waiting));
    }

    //讓使用者翻轉GroupRelation才會用到
    Dictionary<Group, List<Group>> arrowsFromTriggerToWaiting;//key是trigger，trigger指向所有wating
    Dictionary<Group, List<Group>> arrowsFromWaitingToTrigger;//key是waiting，waiting指向所有trigger

    void FillDictionary(Group key,Group value, Dictionary<Group, List<Group>> arrows)
    {
        if (!arrows.ContainsKey(key))
        {
            var list = new List<Group>();
            list.Add(value);
            arrows.Add(key, list);
        }
        else
        {
            var list = arrows[key];
            list.Add(value);
        }
    }

    public void ReBuildArrows()
    {
        arrowsFromTriggerToWaiting = new Dictionary<Group, List<Group>>();
        arrowsFromWaitingToTrigger = new Dictionary<Group, List<Group>>();

        foreach(var relation in groupLinks) {
            var trigger =relation.GetTrigger();
            var waiting = relation.GetWaiting();
            FillDictionary(trigger, waiting, arrowsFromTriggerToWaiting);
            FillDictionary(waiting,trigger, arrowsFromWaitingToTrigger);
        }
       
        Debug.Log("ReBuildArrows");
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

    [SerializeField]
    Game game;

    [SerializeField]
    List<Group> groupList;

    [SerializeField]
    int[] BeginIndex;

    [SerializeField]
    int[] Count;
    public List<Group>  GetGroupList() {
        return GetGroupList(nowFloorIndex);
    }

    List<Group> GetGroupList(int floor) {
        return groupList.GetRange(BeginIndex[floor], Count[floor]);
    }

    [SerializeField]
    int nowFloorIndex;

    void BeforeBuildGroup() {
        Tool.Clear(groupsContainer);
        Tool.Clear(elementsContainer);
        groupList = new List<Group>();
        BeginIndex = new int[voxelBuilder.GetFloor()];
        Count = new int[voxelBuilder.GetFloor()];
    }

    void BeforeBuildLink()
    {
        relationManager.BeforeBuild();
    }

    void AfterBuildLink()
    {
        relationManager.ReBuildArrows();
    }

    public void Build()
    {
        //(1)建立Group(每1層由左下角開始水平掃描)
        BeforeBuildGroup();
        for (var f = 0; f < voxelBuilder.GetFloor(); ++f) {
            BuildGroupInTheFloor(f);
        }

        BeforeBuildLink();
        //(2)每1層作Link(Relation)
        for (var f = 0; f < voxelBuilder.GetFloor(); ++f)
            MakeLinksInTheFloor(f);

        //(3)上下層作Link(Relation)
        for (var f = voxelBuilder.GetFloor() - 1; f >= 1; --f)
            MakeLinkBetween2Floor(f, f - 1);

        //AfterBuildLink();

        //(4)為Element寫入triggerCount和waitings
    }

    void MakeLinksInTheFloor(int floor) {
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
    }

    void MakeLinkBetween2Floor(int floor, int lowerFloor) {
        var groups = GetGroupList(floor);
        var elementList = new List<Element>();
        foreach (var g in groups)
            elementList.AddRange(g.GetElements());
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
        var oldCount = groupList.Count;
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
                groupList.Add(group);
            }
        }
        if (oldCount != groupList.Count) {
            BeginIndex[floor] = oldCount;
            Count[floor] = groupList.Count- oldCount;
        }
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
        var newIndex = nowFloorIndex + offset;
        if (voxelBuilder.IsValidatedFloorIndex(newIndex))
            nowFloorIndex = newIndex;
    }

    public RelationManager relationManager;

}

//https://answers.unity.com/questions/283191/how-do-i-detect-if-a-scene-is-being-loaded-during.html
[InitializeOnLoad]
public static class WhenEditorOpen
{
    //開啟Unity後這裡會執行2次
    //(因為Unity會重新輯譯1次Script檔)
    static WhenEditorOpen()
    {
        Debug.Log("WhenEditorOpen");
        EditorSceneManager.sceneOpened +=SceneOpenedCallback;

        //上面的callback要在開啟Untiy後，重新開啟1個Scene才會觸發
        CallGroupRelationBuilder();
    }

    static void SceneOpenedCallback(UnityEngine.SceneManagement.Scene scene, OpenSceneMode mode)
    {
        CallGroupRelationBuilder();
    }

    static void CallGroupRelationBuilder() {
        var groupRelationBuilder = GameObject.FindObjectOfType<GroupRelationBuilder>();
        if (groupRelationBuilder == null)
            return;
        groupRelationBuilder.relationManager.ReBuildArrows();
    }
}
