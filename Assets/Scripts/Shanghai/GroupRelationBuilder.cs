using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEditor;

//用來記錄相依性
[System.Serializable]
public class Relation {

    [SerializeField]
    Group from;//先

    [SerializeField]
    Group to;//後

    public Relation(Group from, Group to)
    {
        this.from = from;
        this.to = to;
    }
}

[System.Serializable]
public class RelationManager {

    public void BeforeBuild() {
        downToUpLinks = new List<Relation>();
        links = new List<Relation>();
    }

    [SerializeField]
    List<Relation> downToUpLinks;//上下層之間的relation

    public void AddDownToUpLinks(Group from, Group to) {
        downToUpLinks.Add(new Relation(from, to));
    }

    [SerializeField]
    List<Relation> links;//同1層Floor

    Dictionary<Group, Relation> forwardLinks;//Key是from
    Dictionary<Group, Relation> trackBackLinks;//Key是to

    public void ReBuildDictionary()
    {
        Debug.Log("ReBuildDictionary");
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

    public void Build()
    {
        //(1)建立Group(每1層由左下角開始水平掃描)
        BeforeBuildGroup();
        for (var f = 0; f < voxelBuilder.GetFloor(); ++f) {
            BuildGroupInTheFloor(f);
        }

        //(2)每1層作Link(Relation)
        //(3)上下層作Link(Relation)
        //(4)為Element綁定OutputTrigger和InputReceiver
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

    public Relation relation;
    public RelationManager relationManager;

}

//https://answers.unity.com/questions/283191/how-do-i-detect-if-a-scene-is-being-loaded-during.html
[InitializeOnLoad]
public static class WhenEditorOpen
{
    static WhenEditorOpen()
    {
        EditorSceneManager.sceneOpened +=SceneOpenedCallback;
    }

    static void SceneOpenedCallback(UnityEngine.SceneManagement.Scene scene, OpenSceneMode mode)
    {
        var groupRelationBuilder=GameObject.FindObjectOfType<GroupRelationBuilder>();
        if (groupRelationBuilder == null)
            return;
        groupRelationBuilder.relationManager.ReBuildDictionary();
    }
}
