using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomButton : MonoBehaviour
{
    [SerializeField] private RoomBase curRoom;
    public RoomBase CurRoom => curRoom;
    private Button roomBtn;
    private Image roomIcon;

    public bool Empty { get; private set; } = true;
    public bool OwnCombinatedBranch { get; private set; } = false;
    
    public void Init()
    {
        if (roomBtn != null && roomIcon != null) return;
        
        if (transform.parent.name.Contains("And"))
        {
            OwnCombinatedBranch = true;
        }
        roomBtn = GetComponent<Button>();
        roomIcon = GetComponent<Image>();
    }

    private void OnEnable()
    {
        UITool.BtnAddListener(roomBtn,OnRoomBtnClick);
    }

    private void OnDisable()
    {
        UITool.BtnRemoveAllListeners(roomBtn);
    }

    private void OnRoomBtnClick()
    {
        MapManager.Instance.UnlockTargetRoomButton(curRoom);
        
        if(curRoom.isLocked && curRoom != null) return;
        
        //移动到当前房间
        MapManager.Instance.RecordRoomIcon(GetComponent<Image>());

        if (curRoom is NonConsolidatedRoom)
        {
            //Debug.Log($"选择房间，房间信息：当前房间为非组合房间，分支{curRoom.branchNum}，楼层{curRoom.floorNum}，房间类型为{curRoom.roomType}" );
            
            MapManager.Instance.Move(curRoom,0,0);
        }
        else
        {
            var room = curRoom as ConsolidatedRoom;
            // Debug.Log($"选择房间，房间信息：当前房间为组合房间，分支{curRoom.branchNum}，楼层{curRoom.floorNum}，" +
            //           $"房间类型为{curRoom.roomType}，组合分支1{room.fatherRoom01Info.branch}，组合分支2{room.fatherRoom02Info.branch}" );
            
            MapManager.Instance.Move(curRoom ,room.fatherRoom01Info.branch,room.fatherRoom02Info.branch);
        }
        
        //进入当前所选房间
        MapManager.Instance.GoIntoRoom(curRoom);
    }

    public void FillBtnRoom(RoomBase room)
    {
        curRoom = room;
        Empty = false;
    }

    public void Clear()
    {
        curRoom = new NonConsolidatedRoom();
        
        roomIcon.sprite = null;

        Empty = true;
    }

    
}
