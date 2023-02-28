using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 单局游戏内卡牌统计器，并提供对应功能
/// </summary>
[System.Serializable]
public class GameCardsRecorder
{
    [SerializeField] private List<Card> totalCards = new List<Card>();
    public List<Card> TotalCards => totalCards;
    [SerializeField] private List<Card> attackCards = new List<Card>();
    [SerializeField] private List<Card> defenceCards = new List<Card>();
    [SerializeField] private List<Card> skillCards = new List<Card>();
    [SerializeField] private List<Card> abilityCards = new List<Card>();
    
    private Dictionary<CardType, List<Card>> cardsDic = new Dictionary<CardType, List<Card>>();

    /// <summary>
    /// 将卡牌类型和卡牌队列放入字典，原则上只在每次游戏开始时调用
    /// </summary>
    public void Initialize()
    {
        cardsDic.Add(CardType.AttackCard,attackCards);
        cardsDic.Add(CardType.DefenceCard,attackCards);
        cardsDic.Add(CardType.SkillCard,attackCards);
        cardsDic.Add(CardType.AbilityCard,attackCards);
    }

    public void ClearCardsList()
    {
        totalCards.Clear();
        attackCards.Clear();
        defenceCards.Clear();
        skillCards.Clear();
        abilityCards.Clear();
    }

    /// <summary>
    /// 将卡牌放入 - 获得卡牌时
    /// </summary>
    public void PutCard(Card card)
    {
        if (cardsDic.ContainsKey(card.cardType))
        {
            var cardGot = GameObject.Instantiate(card);
            
            cardsDic[card.cardType].Add(cardGot);
            
            totalCards.Add(cardGot);
            
            cardGot.CheckCanUseByNoLight();
        }
        else
        {
            Debug.LogError("添加时传入卡牌类型错误，未在字典中");
        }
    }

    /// <summary>
    /// 从当前牌库中移除卡牌
    /// </summary>
    public void RemoveCard(Card card)
    {
        if(totalCards.Count == 0 ) return;
        
        if (totalCards.Contains(card))
        {
            if (cardsDic.ContainsKey(card.cardType))
            {
                cardsDic[card.cardType].Remove(card);
                
                totalCards.Remove(card);
            }
            else
            {
                Debug.LogError("移除时传入卡牌类型错误，未在字典中");
            }
        }
        else
        {
            Debug.LogError("移除的卡牌未在玩家的牌库中：" + card.cardName);
        }
    }

    /// <summary>
    /// 计算玩家当前拥有的全部卡牌或者某种类型的卡牌
    /// </summary>
    /// <param name="cardType">卡牌类型，默认传入None，None代表着统计所有卡牌数量</param>
    /// <returns>返回对应数量</returns>
    public int CalCardsNum(CardType cardType = CardType.None)
    {
        var cardNum = 0;
        
        if (cardType == CardType.None)
        {
            for (var i = 0; i < totalCards.Count; i++)
            {
                cardNum++;
            }
        }
        else
        {
            if (cardsDic.ContainsKey(cardType))
            {
                for (var i = 0; i < cardsDic[cardType].Count; i++)
                {
                    cardNum++;
                }
            }
            else
            {
                Debug.LogError("传入卡牌类型错误，未在字典中");
            }

        }
       
        return cardNum;
    }

    
}
