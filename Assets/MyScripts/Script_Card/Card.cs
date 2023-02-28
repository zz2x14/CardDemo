using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO:�ֽ����Ƹ�������Ч���ֵúܿ����Ƿ�������ϣ�
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
    /// һЩ��΢����Ŀ��ƣ��������䣩���ڶ�������ֵ����
    /// </summary>
    public int spareNum;
    /// <summary>
    /// ��ǰ��Ƭ����ϵͳ����
    /// </summary>
    public CardLevel cardLevel;
    

    /// <summary>
    /// ��鿨���Ƿ���"�޹�"����
    /// </summary>
    public void CheckCanUseByNoLight()
    {
        canUse = !cardIncidentalTypes.Contains(CardIncidentalType.NoLight);
    }

    /// <summary>
    /// �����Ƿ���"�ȹ�"����
    /// </summary>
    public bool IsCardFirstRound()
    {
        return cardIncidentalTypes.Contains(CardIncidentalType.FirstRound);
    }

    /// <summary>
    /// �����Ƿ���"�ڶ�"����
    /// </summary>
    /// <returns></returns>
    public bool IsBlackHole()
    {
        return cardIncidentalTypes.Contains(CardIncidentalType.BlackHole);
    }

    /// <summary>
    /// ��ǰ���Ƶ�����Ŀ��
    /// </summary>
    /// <returns></returns>
    protected abstract Character[] CardTargets();

    /// <summary>
    /// ��ǰ���Ƶ�Ч��
    /// </summary>
    protected abstract void CardWork(params Character[] targets);

    /// <summary>
    /// ��������
    /// </summary>
    public void UseCard()
    {
        if(!canUse) return;

        CardWork(CardTargets());

        CardUsedIncidentalEffect();
    }

    /// <summary>
    /// ����ʹ��ʱ��Ч�Ŀ��Ƹ���Ч��
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
    /// �жϵ�ǰ�����Ƿ���Ҫָ��Ŀ��ʹ�ã�����Ŀ���������Ŀ�꣩
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


