using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsolidatedRoom : RoomBase
{
    public (int branch, int floor) fatherRoom01Info;
    public (int branch, int floor) fatherRoom02Info;
    
    public void CreateConnected(ConnectedType connectedType,int branchNum01,int floorNum01,int branchNum02,int floorNum02 )
    {
        isConnected = true;
        this.connectedType = connectedType;
        
        fatherRoom01Info = (branchNum01, floorNum01);
        fatherRoom02Info = (branchNum02, floorNum02);

        floorNum = floorNum01;
        
        room01BranchNum = branchNum01;
        room01FloorNum = floorNum01;
        room02BranchNum = branchNum02;
        room02FloorNum = floorNum02;
        
        // if(this.connectedType == ConnectedType.Separation)
        //     Debug.Log("当前楼层为：" + floorNum + ",当前分支类型为：" + this.connectedType);
    }
}
