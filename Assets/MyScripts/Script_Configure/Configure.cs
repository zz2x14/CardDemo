using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Configure
{
    #region Color
    public static Color WhiteWith0Point5Alpha = new (1, 1, 1, 0.5f);
    #endregion

    #region Layer
    public const int Layer_UI = 5;
    #endregion

    #region UI

    /// <summary>
    /// 休息房间移除牌库时的滚屏容器随着每行卡牌增加的高度
    /// </summary>
    public const float CardShowGOEachIncreasedHeight_RestRoomRemove = 300;
    public const int CardShowGOEachLineCardNum_RestRoomRemove = 4;
    #endregion

    //最大手牌数量
    public const int MaxHandNum = 9;
    public const int DefaultDrawCountEachRound = 6;

    //最大关卡数量
    public const int MaxLevelNum = 3;
    
    //除Boss房间外的楼层数量
    public const int MaxFloorCount = 14;

    //分支数量范围
    public const int MinBranchCount = 3;
    public const int MaxBranchCount = 4;
    
    //单局战斗最大敌人数量
    public const int MaxEnemyNumInBattle = 5;

    //地图每次的最大刷新次数
    public const int maxFreshNum = 30;

    //第一关卡中商店的数量要求
    public const int MinShopNumInFirstLevel = 1;
    public const int MaxShopNumInFirstLevel = 4;
    public const int MinShopNumInFirstLevelFifthState = 1;
    public const int MaxShopNumInFirstLevelFifthState = 2;
    //第一关卡中随机事件的数量要求
    public const int MinRandomEventNumInFirstLevel = 5;
    public const int MaxRandomEventNumInFirstLevel = 9;
    //第一关卡中精英敌人战斗的数量要求
    public const int MinEliteEnemyNumInFirstLevel = 3;
    public const int MaxEliteEnemyNumInFirstLevel = 5;
    //第一关卡休息地的数量要求
    public const int MinRestNumInFirstLevel = 2;
    public const int MaxRestNumInFirstLevel = 4;

    //休息房间回复生命值百分比
    public const float RestRoomRestorePercentage = 0.25f;

    //Boss出现标准率
    public const int BossMaxRate = 100;
    //玩家能够拥有的时空原石默认最大数量
    public const int SpaceStoneDefaultMaxOwnedNum = 9;
}
