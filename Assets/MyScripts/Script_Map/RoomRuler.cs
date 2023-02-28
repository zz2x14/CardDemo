using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Ruler/NewRoomRuler",fileName = "NewRoomRuler")]
public class RoomRuler : ScriptableObject
{
    [Header("房间楼层")]
    public int levelNum;
    public int floorBegin;
    public int floorEnd;
    public RoomFloorType roomFloorType;
    [Header("房间类型权重")]
    [Range(0,100)] public int enemyBattaleRate;
    [Range(0,100)] public int eliteEnemyBattaleRate;
    [Range(0, 100)] public int shopRate;
    [Range(0, 100)] public int randomEvenetRate;
    [Range(0, 100)] public int restRate;
    [Range(0, 100)] public int tearsureRate;
    
    [Header("时空原石商店概率")] 
    [Range(0, 100)] public int spacetimeStoneShopRate;
    
    [Header("楼层对应敌人组合权重")] 
    [Range(0, 100)] public int levelCRate;
    [Range(0, 100)] public int levelBRate;
    [Range(0, 100)] public int levelARate;
    
    private readonly Dictionary<RoomType, int> roomWeightDic = new Dictionary<RoomType, int>();

    private readonly Dictionary<EnemyCombinationLevel, int> enemyCombinationLevelWeightDic =
        new Dictionary<EnemyCombinationLevel, int>();

    private void OnEnable()
    {
        InitializeDic();
    }
    
    //初始化两个字典
    private void InitializeDic()
    {
        if(enemyBattaleRate > 0 )
            roomWeightDic.Add(RoomType.EnemyRoom,enemyBattaleRate);
        if(eliteEnemyBattaleRate > 0 )
            roomWeightDic.Add(RoomType.EliteEnemyRoom,eliteEnemyBattaleRate);
        if(shopRate > 0 )
            roomWeightDic.Add(RoomType.ShopRoom,shopRate);
        if(randomEvenetRate > 0 )
            roomWeightDic.Add(RoomType.RandomEventRoom,randomEvenetRate);
        if(restRate > 0 )
            roomWeightDic.Add(RoomType.RestRoom,restRate);
        if(tearsureRate > 0 )
            roomWeightDic.Add(RoomType.TreasureRoom,tearsureRate);
        
        if(levelCRate > 0 )
            enemyCombinationLevelWeightDic.Add(EnemyCombinationLevel.C,levelCRate);
        if(levelBRate > 0 )
            enemyCombinationLevelWeightDic.Add(EnemyCombinationLevel.B,levelCRate);
        if(levelARate > 0 )
            enemyCombinationLevelWeightDic.Add(EnemyCombinationLevel.A,levelCRate);
    }

    /// <summary>
    /// 根据规则配置权重随机房间类型
    /// </summary>
    public RoomType GetBaseRoomType()
    {
        var random = Random.Range(0, 101);
        var counter = 0;
        foreach (var temp in roomWeightDic)
        {
            counter += temp.Value;
            if (random <= counter)
            {
                // Debug.Log(temp.Key);
                return temp.Key;
            }
        }
        
        Debug.LogError("获取房间类型失败");
        return 0;
    }
    
    /// <summary>
    /// 根据规则配置权重随机获得敌人组合
    /// </summary>
    public EnemyCombinationLevel GetEnemyCombinationLevel()
    {
        var random = Random.Range(0, 101);
        var counter = 0;
        foreach (var temp in enemyCombinationLevelWeightDic)
        {
            counter += temp.Value;
            if (random <= counter)
            {
                // Debug.Log(temp.Key);
                return temp.Key;
            }
        }
        
        Debug.LogError("获取敌人组合等级失败");
        return 0;
    }
}


