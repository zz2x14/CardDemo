using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO:现将卡牌附加属性效果分得很开，是否进行整合？
public abstract class Card : ScriptableObject
{
    public int cardId;
    public string cardName;
    public int cardCost;
    public bool canUse = true;
    public CardType cardType;
    public CardWorkPattern workPattern;
    public List<CardIncidentalType> cardIncidentalTypes = new List<CardIncidentalType>();
    /// <summary>
    /// 一些稍微特殊的卡牌，（用于其）存在独立的数值需求
    /// </summary>
    public int spareNum;
    /// <summary>
    /// 当前卡片背后系统评级
    /// </summary>
    public CardLevel cardLevel;
    

    /// <summary>
    /// 检查卡牌是否有"无光"特性
    /// </summary>
    public void CheckCanUseByNoLight()
    {
        canUse = !cardIncidentalTypes.Contains(CardIncidentalType.NoLight);
    }

    /// <summary>
    /// 卡牌是否有"先攻"特性
    /// </summary>
    public bool IsCardFirstRound()
    {
        return cardIncidentalTypes.Contains(CardIncidentalType.FirstRound);
    }

    /// <summary>
    /// 卡牌是否有"黑洞"特性
    /// </summary>
    /// <returns></returns>
    public bool IsBlackHole()
    {
        return cardIncidentalTypes.Contains(CardIncidentalType.BlackHole);
    }

    /// <summary>
    /// 当前卡牌的作用目标
    /// </summary>
    /// <returns></returns>
    protected abstract Character[] CardTargets();

    /// <summary>
    /// 当前卡牌的效果
    /// </summary>
    protected abstract void CardWork(params Character[] targets);

    /// <summary>
    /// 触发卡牌
    /// </summary>
    public void UseCard()
    {
        if(!canUse) return;

        CardWork(CardTargets());

        CardUsedIncidentalEffect();
    }

    /// <summary>
    /// 部分使用时生效的卡牌附加效果
    /// </summary>
    public void CardUsedIncidentalEffect()
    {
        if (cardIncidentalTypes.Count == 0) return;

        foreach (var card in cardIncidentalTypes)
        {
            switch (card)
            {
                case CardIncidentalType.Passing :
                    CardManager.Instance.CardPassingEffect(this);
                    break;
                case CardIncidentalType.SpaceJump:
                    BattleManager.Instance.RecordNextRoundOwedEnergy(cardCost);
                    break;
                case CardIncidentalType.Backtrack:
                    CardManager.Instance.ReturnDraw(this);
                    break;
            }
        }
    }

    /// <summary>
    /// 判断当前卡牌是否需要指定目标使用（单个目标或者相邻目标）
    /// </summary>
    public bool NeedPointedTarget()
    {
        return workPattern is CardWorkPattern.ForPointed or CardWorkPattern.ForAdjacent;
    }

   
}

public enum CardType
{
    None,
    AttackCard,
    DefenceCard,
    SkillCard,
    AbilityCard,
}

public enum CardIncidentalType
{
    Passing,
    SpaceJump,
    FirstRound,
    Backtrack,
    BlackHole,
    NoLight
}

public enum CardWorkPattern
{
    None,
    ForSelf,
    ForPointed,
    ForAdjacent,
    ForRandomEnemy,
    ForAllEnemy,
    ForAllCharacter
}


