using System;
using UnityEngine;

/// <summary>
/// TODO:分支类型是否有分为交汇点和分离点的必要？
/// </summary>
[Serializable]
public class RoomBase
{
    //若branchNum为0则说明该房间为两条路线合并后的共有房间
    public int branchNum;
    public int floorNum;
    public RoomType roomType;
    public RoomFloorType roomFloorType;
    public bool isLocked = true;
    [Space]
    public bool isConnected = false;
    public ConnectedType connectedType = ConnectedType.None;
    
    [Header("仅供查看验证")]
    public int room01BranchNum;
    public int room01FloorNum;
    public int room02BranchNum;
    public int room02FloorNum;

    public void InitializeRoom()
    {
        roomFloorType = floorNum switch
        {
            1 => RoomFloorType.FirstState,
            > 1 and <= 5 => RoomFloorType.SecondState,
            > 5 and <= 9 => RoomFloorType.ThirdState,
            10 => RoomFloorType.FourthState,
            > 10 and <= 14 => RoomFloorType.FifthState,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}

public enum RoomType
{
    EnemyRoom,
    EliteEnemyRoom,
    ShopRoom,
    RandomEventRoom,
    RestRoom,
    TreasureRoom
}

public enum RoomFloorType
{
    FirstState,
    SecondState,
    ThirdState,
    FourthState,
    FifthState
}

public enum ConnectedType
{
    None,
    Intersection,
    Middle,
    Separation
}
