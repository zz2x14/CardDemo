using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class RoomCalculator
{
    [SerializeField] private List<RoomBase> floor_1 = new List<RoomBase>();
    [SerializeField] private List<RoomBase> floor_2 = new List<RoomBase>();
    [SerializeField] private List<RoomBase> floor_3 = new List<RoomBase>();
    [SerializeField] private List<RoomBase> floor_4 = new List<RoomBase>();
    [SerializeField] private List<RoomBase> floor_5 = new List<RoomBase>();
    [SerializeField] private List<RoomBase> floor_6 = new List<RoomBase>();
    [SerializeField] private List<RoomBase> floor_7 = new List<RoomBase>();
    [SerializeField] private List<RoomBase> floor_8 = new List<RoomBase>();
    [SerializeField] private List<RoomBase> floor_9 = new List<RoomBase>();
    [SerializeField] private List<RoomBase> floor_10 = new List<RoomBase>();
    [SerializeField] private List<RoomBase> floor_11 = new List<RoomBase>();
    [SerializeField] private List<RoomBase> floor_12 = new List<RoomBase>();
    [SerializeField] private List<RoomBase> floor_13 = new List<RoomBase>();
    [SerializeField] private List<RoomBase> floor_14 = new List<RoomBase>();

    public List<RoomBase> totalRoom = new List<RoomBase>();

    private readonly Dictionary<int, List<RoomBase>> roomListDic = new Dictionary<int, List<RoomBase>>();

    public void Initialize()
    {
        roomListDic.Clear();
        
        roomListDic.Add(1,floor_1);
        roomListDic.Add(2,floor_2);
        roomListDic.Add(3,floor_3);
        roomListDic.Add(4,floor_4);
        roomListDic.Add(5,floor_5);
        roomListDic.Add(6,floor_6);
        roomListDic.Add(7,floor_7);
        roomListDic.Add(8,floor_8);
        roomListDic.Add(9,floor_9);
        roomListDic.Add(10,floor_10);
        roomListDic.Add(11,floor_11);
        roomListDic.Add(12,floor_12);
        roomListDic.Add(13,floor_13);
        roomListDic.Add(14,floor_14);
    }

    public void UnlockFirstFloorRooms()
    {
        foreach (var room in floor_1)
        {
            room.isLocked = false;
        }
    }

    /// <summary>
    /// 将房间放入对应的楼层中
    /// </summary>
    /// <param name="room"></param>
    public void PutRoomIntoList(RoomBase room)
    {
        if (roomListDic.ContainsKey(room.floorNum))
        {
            totalRoom.Add(room);
            
            roomListDic[room.floorNum].Add(room);
        }
        else
        {
            Debug.LogError("传入房间楼层错误:" + room.floorNum);
        }
    }

    public void ClearRoomList()
    {
        totalRoom.Clear();
        
        floor_1.Clear();
        floor_2.Clear();
        floor_3.Clear();
        floor_4.Clear();
        floor_5.Clear();
        floor_6.Clear();
        floor_7.Clear();
        floor_8.Clear();
        floor_9.Clear();
        floor_10.Clear();
        floor_11.Clear();
        floor_12.Clear();
        floor_13.Clear();
        floor_14.Clear();
    }
    
    /// <summary>
    /// 计算目标楼层的目标房间类型的数量
    /// </summary>
    /// <param name="targetRoomType">目标房间类型</param>
    /// <param name="roomNum">数量接收</param>
    /// <param name="floorNums">目标楼层</param>
    public void CalTargetRoomNumInTargetFloors(RoomType targetRoomType,ref int roomNum,params int[] floorNums)
    {
        foreach (var floorNum in floorNums)
        {
            if (roomListDic.ContainsKey(floorNum))
            {
                for (var i = 0; i < roomListDic[floorNum].Count; i++)
                {
                    if (roomListDic[floorNum][i].roomType == targetRoomType)
                    {
                        roomNum++;
                    }
                }
            }
            else
            {
                Debug.LogError("传入房间楼层错误"); 
            }
        }
    }

    /// <summary>
    /// 计算目标房间类型的总数量
    /// </summary>
    /// <param name="targetRoomType">目标房间类型</param>
    /// <param name="roomNum">数量接收</param>
    public void CalTargetRoomNumInTotalRoom(RoomType targetRoomType,ref int roomNum)
    {
        roomNum += totalRoom.Count(room => room.roomType == targetRoomType);
    }

    /// <summary>
    /// 计算当前房间所有数量
    /// </summary>
    /// <returns></returns>
    public int CalAllRoomNum()
    {
        return totalRoom.Count;
    }

    //进入到下一层后关闭上一层的所有房间
    public void CloseLastFloorRooms(int curFloor)
    {
        if (curFloor <= 1) return;
        
        foreach (var lastFloorRoom in roomListDic[curFloor - 1])
        {
            lastFloorRoom.isLocked = true;
        }
    }
}
