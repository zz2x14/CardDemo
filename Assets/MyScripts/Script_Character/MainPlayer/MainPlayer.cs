using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MainPlayer : Character
{
    private MainPlayerInfo mainPlayerInfo;
    public List<Card> DefaultCardList => mainPlayerInfo.playerDefaultCardsLibrary.defaultCards;
    public MainPlayerEnum MainPlayerOccupation => mainPlayerInfo.mainPlayerEnum;

    protected virtual void Awake()
    {
        mainPlayerInfo = baseInfo as MainPlayerInfo;
    }

    /// <summary>
    /// 初始化能量
    /// </summary>
    public void InitializeEnergy(out int maxEnergy)
    {
        maxEnergy = mainPlayerInfo.defaultEnergy;
    }

    /// <summary>
    /// 出牌
    /// </summary>
    public virtual void PlayHand(Card card)
    {
        
    }
    
}

public enum MainPlayerEnum
{
    StreetFighter,
    Neutral
}
