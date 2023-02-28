using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Branch
{
    //public int floorCount = Configure.MaxFloorCount;
    public List<RoomBase> rooms = new List<RoomBase>();
}
