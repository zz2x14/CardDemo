using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BattleCard : Card
{
    [Header("战斗卡牌属性")]
    [SerializeField] protected MainPlayerEnum cardOwner;
    [SerializeField] protected bool isUpgraded = false;
    [SerializeField] protected BattleCard upgradedCard;
    
    [Header("卡牌携带Buff")]
    [SerializeField] private List<CharacterBuff> buffList = new List<CharacterBuff>();
    [SerializeField] private List<int> buffNumList = new List<int>();
    protected readonly Dictionary<CharacterBuff, int> buffsNumsDic = new Dictionary<CharacterBuff, int>();

    protected MainPlayer owner;
    
    private void OnEnable()
    {
        if(buffNumList.Count == 0) return;

        if (buffNumList.Count == buffNumList.Count)
        {
            for (var i = 0; i < buffList.Count; i++)
            {
                buffsNumsDic.Add(buffList[i],buffNumList[i]);
            }
        }
        else
        {
            DebugTool.MyDebugError($"卡牌行为buff和buff层数初始化时出错，两者数量不相等");
        }
    }

    /// <summary>
    /// 传入玩家角色
    /// </summary>
    public void GetOwner(MainPlayer mainPlayer)
    {
        owner = mainPlayer;
    }

    protected override Character[] CardTargets()
    {
        return null;
    }

    protected override void CardWork(params Character[] targets)
    {
        
    }

    
}
