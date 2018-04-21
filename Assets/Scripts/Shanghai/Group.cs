using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GroupState {
    ShuffleNotUsing,//還沒放置過
    ShuffleUsing,//至少有1個被放置了
    GameReady,//整條都洗牌完了
    GameFinish//玩家已經清掉整條
}
public class Group : MonoBehaviour
{
    public void CheckIfHasFloorLink()
    {
        foreach (var e in elements) {
            if (e.triggerCount > 0) {
                hasFloorLink = true;
                return;
            }
        }
        hasFloorLink = false;
    }

    [SerializeField]
    GroupRelationBuilder groupRelationBuilder;
    public void SetGroupRelationBuilder(GroupRelationBuilder builder) { groupRelationBuilder = builder; }

    public bool hasFloorLink;
    public bool hasInputArrow;
    public bool isInSuffleList;

    public bool hasOutputArrow;

    public float GetPosX() { return transform.position.x; }
    public Element GetHeadElement() { return elements[0]; }
    public Element GetTailElement() { return elements[elements.Length - 1]; }

    public GroupState state;

    bool isFirstSuffle;//是不是一開局的洗牌？
    int inGameLeftIndex, inGameRightIndex;//記錄遊戲進行中的左右2端
    public int shuffleLeftIndex, shuffleRightIndex;
    public int shuffeUseCount;
    public int DebugStartIndex;
    public void AddUseCounter() { ++shuffeUseCount; }
    public void MinusUseCounter() { --shuffeUseCount; }

    public int GetShuffeNotUseCount(){return GetSuffleMaxCount()-shuffeUseCount;}

    [SerializeField]
    Element[] elements;
    public void AddElements(Element[] element){
        elements = element;
        for (var i = 0; i < elements.Length; ++i)
        {
            var e = element[i];
            e.group = this;
            e.indexInGroup = i;
        }   
    }

    public void AddToShufflingSet()
    {
        groupRelationBuilder.AddToShufflingSet(this);
    }

    public Element[] GetElements(){return elements;}

    public int xBegin,xEnd;
    public int y;
    public int floor;
    public void Set(int floor, int y, int xBegin, int xEnd)
    {
        this.floor = floor;
        this.y = y;
        this.xBegin = xBegin;
        this.xEnd = xEnd;
        name = floor + "," + y + ", [" + xBegin + " to "  + xEnd+"]";
    }

    public void Setpos(Voxel voxel)
    { transform.localPosition = voxel.transform.localPosition; }

    public void PickOne()
    {
        //如果elment裡有outputTrigger就發送
    }

    public Vector3[] GetRect() {
        var BeginElement = elements[0].transform;
        var EndElement = elements[elements.Length-1].transform;
        var offsetX = 0.5f*VoxelBuilder.xUnit*Vector3.right;
        var offsetY = 0.5f * VoxelBuilder.yUnit * Vector3.forward;
        return new Vector3[] {//從左下角開始逆時鐘方向轉1圈
            BeginElement.position- offsetX- offsetY,
            EndElement.position+ offsetX- offsetY,
            EndElement.position+ offsetX+ offsetY,
            BeginElement.position- offsetX+ offsetY
        };
    }

    int GetSuffleMaxCount()
    {
        if (isFirstSuffle)
            return elements.Length;
        else
            return (inGameRightIndex - inGameLeftIndex)/2 + 1;//因為每個之間隔2格
    }

    public bool IsSuffleFinish() {
        var result = shuffeUseCount == GetSuffleMaxCount();
        if (result)
            state = GroupState.GameReady;
        return result;
    }

    public void SetIsFirstShuffle(bool b){ isFirstSuffle = b; }

    //如果沒有inputArrow depth就是0
    //有inputArrow的話
    //G1<-G2<-G3
    //像G2的Depth就是1
    //G1的Dpeth就是2
    public int depth;

    public int downToUpArrowDepth;

    public void BeforeShuffle()
    {
        isInSuffleList = false;
        shuffeUseCount = 0;
        state = GroupState.ShuffleNotUsing;
    }

    public bool CanSetElement() {
        var groupNotUse = state == GroupState.ShuffleNotUsing;
        if (hasInputArrow)//如果是有同層相依性的group
        {
            var groupRelation = groupRelationBuilder.GetGroupInputArrows(this)[0];
            if (groupNotUse)
            {
                //檢查端點
                if (groupRelation.IsRightSideLink())
                    return GetTailElement().CanUse();
                else
                    return GetHeadElement().CanUse();
            }
            else {
                //檢查端點
                var nextElement = groupRelation.IsRightSideLink()?elements[shuffleLeftIndex]: elements[shuffleRightIndex];
                return nextElement.CanUse();
            }
        }
        else
        {
            if (groupNotUse)
            {
                //只要有可以用的元素就行了
                return HasAnyElementCanUse();
            }
            else
            {
                //檢查端點左右2端
                return LeftOrRightElementCanUse();
            }
        }
    }

    
    public Element PickElementInGroup() {
        pickElement=null;
        var groupNotUse = state == GroupState.ShuffleNotUsing;
        if (hasInputArrow)//如果是有同層相依性的group
        {
            var groupRelation = groupRelationBuilder.GetGroupInputArrows(this)[0];
            if (groupNotUse)
            {
                //挑端點
                if (groupRelation.IsRightSideLink())
                {
                    pickElement = GetTailElement();
                    shuffleLeftIndex = elements.Length - 2;
                }
                else
                {
                    pickElement= GetHeadElement();
                    shuffleRightIndex = 1;
                }      
            }
            else
            {
                //挑端點
                pickElement = groupRelation.IsRightSideLink() ? elements[shuffleLeftIndex] : elements[shuffleRightIndex];
                if (groupRelation.IsRightSideLink())
                    --shuffleLeftIndex;//往左移動
                else
                    ++shuffleRightIndex;//往右移動
            }
        }
        else
        {
            if (groupNotUse)
            {
                //從ready的elments中随機挑出1個element
                var canUseElements = GetCanUseElements();

                var index = Random.Range(0, canUseElements.Count);
                pickElement = canUseElements[index];
                var indexInGroup = pickElement.indexInGroup;//這才是正確的索引
                shuffleLeftIndex = indexInGroup - 1;
                shuffleRightIndex = indexInGroup + 1;
                DebugStartIndex = indexInGroup;
            }
            else
            {
                //nowPickIndex左/右挑1個element(要ready的才行)
                pickElement = GetLeftOrRightElementCanUse();
            }
        }
        //執行過1次就切換狀態
        state = GroupState.ShuffleUsing;

        pickElement.SetUse();
        return pickElement;
    }

    Element pickElement;
    GroupState memoryState;
    int memoryShuffleLeftIndex;
    int memoryShuffleRightIndex;

    public void MemoryState()
    {
        memoryState = state;
        memoryShuffleLeftIndex=shuffleLeftIndex;
        memoryShuffleRightIndex=shuffleRightIndex;
    }

    public void RollBack()
    {
        state=memoryState;
        shuffleLeftIndex=memoryShuffleLeftIndex;
        shuffleRightIndex=memoryShuffleRightIndex;

        pickElement.SetNotUse();
    }

    bool IsValidIndex(int index)
    {
        return index >= 0 && index < elements.Length;
    }

    bool LeftOrRightElementCanUse()
    {
        var validLeft =IsValidIndex(shuffleLeftIndex);
        var validRight = IsValidIndex(shuffleRightIndex);

        if (validLeft && validRight)
            return elements[shuffleLeftIndex].CanUse() || elements[shuffleRightIndex].CanUse();
        else if (validLeft)
            return elements[shuffleLeftIndex].CanUse();
        else if (validRight)
            return elements[shuffleRightIndex].CanUse();
        else
            return false;
    }

    Element GetLeftOrRightElementCanUse()
    {
        Element target = null;
        var validLeft = IsValidIndex(shuffleLeftIndex);
        var validRight = IsValidIndex(shuffleRightIndex);

        if (validLeft && validRight)
        {
            var leftElement = elements[shuffleLeftIndex];
            var rightElement = elements[shuffleRightIndex];

            if (leftElement.CanUse() && rightElement.CanUse())
            {
                var randomValue = Random.Range(0, 2);
                if (randomValue == 0)
                {
                    target = leftElement;
                    shuffleLeftIndex = shuffleLeftIndex - 1;
                }
                else
                {
                    target = rightElement;
                    shuffleRightIndex = shuffleRightIndex + 1;
                }
            }
            else if (leftElement.CanUse())
            {
                target = leftElement;
                shuffleLeftIndex = shuffleLeftIndex - 1;
            }
            else
            {
                target = rightElement;
                shuffleRightIndex = shuffleRightIndex + 1;
            }
        }
        else if (validLeft)
        {
            var leftElement = elements[shuffleLeftIndex];
            target = leftElement;
            shuffleLeftIndex = shuffleLeftIndex - 1;
        }
        else if (validRight)
        {
            var rightElement = elements[shuffleRightIndex];
            target = rightElement;
            shuffleRightIndex = shuffleRightIndex + 1;
        }

        return target;
    }

    bool HasAnyElementCanUse()
    {
        foreach (var e in elements)
        {
            if (e.CanUse())
                return true;
        }
        return false;
    }

    List<Element> temp=new List<Element>();
    List<Element> GetCanUseElements()
    {
        temp.Clear();
        foreach (var e in elements)
        {
            if (e.CanUse())
                temp.Add(e);
        }
        return temp;
    }
}
