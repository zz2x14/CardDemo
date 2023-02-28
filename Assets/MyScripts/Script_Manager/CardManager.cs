using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Configure;

/// <summary>
/// 卡牌管理器，提供卡牌相关功能
/// </summary>
public class CardManager : PersistentSingleton<CardManager>
{
    [Header("私有仅供查看")]
    
    [Header("所有已解锁卡牌")]
    [SerializeField] private List<Card> cardLibraryUnlock = new List<Card>();
    public List<Card> CardLibraryUnlock => cardLibraryUnlock;
    
    [Header("单局全局卡牌")]
    [SerializeField] private List<Card> totalCards = new List<Card>();
    public List<Card> TotalCards => totalCards;
    [SerializeField] private List<Card> attackCards = new List<Card>();
    [SerializeField] private List<Card> defenceCards = new List<Card>();
    [SerializeField] private List<Card> skillCards = new List<Card>();
    [SerializeField] private List<Card> abilityCards = new List<Card>();
    private Dictionary<CardType, List<Card>> cardsDic = new Dictionary<CardType, List<Card>>();
    
    [Header("战斗中卡牌")]
    [SerializeField] private List<Card> availableCards = new List<Card>(); //仍可使用的牌（指还可以再进抽牌堆中抽到的牌）
    [SerializeField] private List<Card> draw = new List<Card>();//抽牌堆
    [SerializeField] private List<Card> hand = new List<Card>();//手牌堆
    [SerializeField] private List<Card> discardeCards = new List<Card>(); //弃牌堆
    [SerializeField] private List<Card> passingCards = new List<Card>();//流逝牌堆
    [SerializeField] private List<Card> thisRoundUsedCards = new List<Card>(); //这回合使用过的卡牌
    [SerializeField] private List<Card> thisBattleUsedCards = new List<Card>();//这场战斗使用过的卡牌
    [SerializeField] private List<Card> rubbishCards = new List<Card>(); //裂缝牌流逝后进入的牌堆
    private Card lastPlayCard;
    private Card curPlayerCard;
    
    private int drawCount; //当次抓牌数量
    public int CurHandNum => hand.Count;

    protected override void Awake()
    {
        base.Awake();

        Initialize();
    }

    #region 单局全局卡牌功能
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
    #endregion

    #region 战斗中卡牌功能
    /// <summary>
    /// 还原清空
    /// </summary>
    public void ClearReCorder()
    {
        availableCards.Clear();
        draw.Clear();
        hand.Clear();
        hand.Clear();
        discardeCards.Clear();
        passingCards.Clear();
        thisRoundUsedCards.Clear();
        thisBattleUsedCards.Clear();
        rubbishCards.Clear();
    }

    /// <summary>
    /// 每次战斗开始第一次洗牌
    /// </summary>
    /// <param name="totalCards">当前拥有的所有牌</param>
    public void FirstShuffle()
    {
        availableCards.Clear();
        hand.Clear();
        discardeCards.Clear();
        passingCards.Clear();
        
        rubbishCards.Clear();
        
        ClearThisRoundCards();
        thisBattleUsedCards.Clear();

        //TODO:SO只作为数据模板时一定克隆复制区分，（作为模板时）尽量不要直接使用SO的类型
        var container = totalCards.Select(card => Instantiate(card)).ToList();

        ReplaceAndFill(container,availableCards);
        ReplaceAndFill(availableCards,draw);
        
        FillCardsWithFirstRound();
    }

    //将先攻特性的卡牌先填充近手牌
    private void FillCardsWithFirstRound()
    {
        if(draw.Count == 0) return;

        for (var i = 0; i < draw.Count; i++)
        {
            if (draw[i].IsCardFirstRound())
            {
                //将手牌放入手牌堆中
                DrawCard(draw[i]);
                //本回合需要抓牌数量减少1
                drawCount -= 1;
            }
        }
    }
    
    /// <summary>
    /// 抽牌堆为空时的洗牌
    /// </summary>
    public void Shuffle()
    {
        discardeCards.Clear();
        
        ReplaceAndFill(availableCards,draw);
        
        //Debug.Log("洗牌");
    }

    /// <summary>
    /// 获得抓牌数量
    /// </summary>
    public void GetDrawCount(int thisDrawCount)
    {
        //如果计算抓牌数量后仍小于0，则归0
        if (drawCount + thisDrawCount < 0)
        {
            drawCount = 0;
        }
        else
        {
            drawCount += thisDrawCount;
        }

        //Debug.Log("抓牌数量为：" + drawCount);
    }

    /// <summary>
    /// 将抓牌数归0
    /// </summary>
    public void ClearDrawCount()
    {
        drawCount = 0;
    }

    /// <summary>
    /// 从牌堆抓牌
    /// </summary>
    public void DrawCard()
    {
        if (drawCount <= 0) return;

        for (var i = 0; i < drawCount; i++)
        {
            if (CurHandNum < MaxHandNum)
            {
                //抽牌堆不够时洗牌
                if (draw.Count == 0)
                {
                    Shuffle();
                }
                
                var cardNum = Random.Range(0, draw.Count);

                for (var j = 0; j < draw.Count; j++)
                {
                    if (j == cardNum)
                    {
                        var drawCard = draw[j];
                        
                        if (drawCard is BattleCard battleCard)
                        {
                            battleCard.GetOwner(MainPlayerManager.Instance.CurMainPlayer);
                        }
                        
                        //Debug.Log("当前手牌数量：" + CurHandNum);
                        
                        hand.Add(drawCard);
                        
                        //进入手牌后从可用牌中移除 - 避免洗牌时还会抽到该牌 - 抽完牌后重置回可用牌中
                        availableCards.Remove(drawCard);
                        
                        draw.Remove(drawCard);
                    }
                }
            }
            else
            {
                Debug.Log("当前手牌已满");
            }
        }
        
        //抽完牌后将手牌中的牌回可用牌中
        foreach (var handCard in hand)
        {
            availableCards.Add(handCard);
        }
    }

    /// <summary>
    /// 抓取指定的卡牌 - 将指定卡牌放入手牌中
    /// </summary>
    /// <param name="targetCard">目标卡牌</param>
    public void DrawCard(Card targetCard)
    {
        if(CurHandNum == MaxHandNum) return;

        //为能非可用的牌 - 比如从流逝牌堆中、获得临时卡牌
        if (!availableCards.Contains(targetCard))
        {
            //从流逝牌中唤回来的牌
            if (passingCards.Contains(targetCard))
            {
                passingCards.Remove(targetCard);
            }
            
            availableCards.Add(targetCard);
        }
        //从弃牌堆中抓来的牌
        else if (discardeCards.Contains(targetCard))
        {
            discardeCards.Remove(targetCard);
        }
        //从抽牌堆正常获得来的
        else if(draw.Contains(targetCard))
        {
            draw.Remove(targetCard);
        }
        
        hand.Add(targetCard);
    }

    /// <summary>
    /// 使用卡牌时的整合流程
    /// </summary>
    public void UseHandProcess(Card card)
    {
        UseHand(card);
        
        RecordUsedCardInThisRound(card);
        RecordUsedCardInThisBattle(card);
        RecordUsedCard(card);
    }
    
    /// <summary>
    /// 使用卡牌
    /// </summary>
    /// TODO：从非手牌中打出的功能未写
    private void UseHand(Card card)
    {
        //从手牌中正常打出
        if (hand.Contains(card))
        {
            card.UseCard();
            
            DiscardCard(card);
        }
    }

    /// <summary>
    /// 回合结束时的弃牌
    /// </summary>
    /// <param name="keepCount">保留数量，默认为0</param>
    public void DiscardCardWhenRoundOver(params Card[] keepCards)
    {
        if (keepCards.Length > hand.Count)
        {
            Debug.LogError("保留卡牌数量大于当前手牌数量");
            return;
        }
        
        //最终保留在手牌的卡
        var keepHand = new List<Card>();
        
        if (keepCards.Length > 0)
        {
            foreach (var keepCard in keepCards)
            {
                if (!hand.Contains(keepCard))
                {
                    Debug.LogError("手牌中没有该牌供保留：" + keepCard.cardName);
                }
                else
                {
                    keepHand.Add(keepCard);
                    
                    hand.Remove(keepCard);
                }
            }
        }
        
        foreach (var discardedCard in hand)
        {
            discardeCards.Add(discardedCard);
        }
        
        hand = keepHand;
    }

    /// <summary>
    /// 非回合结束时的弃牌
    /// </summary>
    /// <param name="intoDiscardeCards">是否进入丢牌堆，默认为true</param>
    public void DiscardCard(Card card,bool intoDiscardeCards = true)
   {
       if(CurHandNum == 0) return;

       if (!hand.Contains(card)) return;
       hand.Remove(card);

       if(intoDiscardeCards)
           discardeCards.Add(card);
       
       //TODO：因有流逝效果的原因，暂时不进行此处的Debug，后续可能考虑改进
       // else
       // {
       //     Debug.LogError("手牌未拥有当前卡牌：" + card.cardName);
       // }
   }

    /// <summary>
    /// 卡牌黑洞效果生效 TODO：将其放在回合结束时检测，后续还会有其它在该阶段检测的效果
    /// </summary>
    public void CardBlackHole()
    {
        if (hand.Count == 0) return;

        var blackHoleList = hand.Where(card => card.IsBlackHole()).ToList();

        for (var i = 0; i < blackHoleList.Count; i++)
        {
            CardPassingEffect(blackHoleList[i]);
        }
    }

    /// <summary>
    /// 卡牌流逝作用生效
    /// </summary>
    public void CardPassingEffect(Card card)
   {
       if (card.cardIncidentalTypes.Count == 0)
       {
           Debug.LogError("该卡牌未拥有任何附加效果：" + card.cardName);

           if (card.cardIncidentalTypes.Contains(CardIncidentalType.Passing))
           {
               Debug.LogError("该卡牌未拥有流逝效果：" + card.cardName);
               return;
           }
       }
       
       //仍存在手牌时从手牌中移除
       if (hand.Contains(card))
       {
           DiscardCard(card,false);
       }
       //从抽牌堆中移除
       else if (draw.Contains(card))
       {
           draw.Remove(card);
       }
       //从弃牌堆中移除
       else if (discardeCards.Contains(card))
       {
           discardeCards.Remove(card);
       }
       
       availableCards.Remove(card);
       passingCards.Add(card);
   }

    /// <summary>
    /// 回到抽牌堆
    /// </summary>
    public void ReturnDraw(Card card)
    {
        //从流逝牌堆中获取牌返回抽牌堆
        if (passingCards.Contains(card))
        {
            passingCards.Remove(card);
            
            availableCards.Add(card);
        }
        //从弃牌堆中获取牌放回抽牌堆
        else if(discardeCards.Contains(card))
        {
            discardeCards.Remove(card);
        }
        //手牌中将牌放回抽牌堆
        else if (hand.Contains(card))
        {
            DiscardCard(card,false);
        }
        
        draw.Add(card);
    }
    
    /// <summary>
    /// 记录上一张和当前使用手牌
    /// </summary>
    private void RecordUsedCard(Card card)
    {
        if (curPlayerCard != null)
            lastPlayCard = curPlayerCard;
            
        curPlayerCard = card;
    }

    /// <summary>
    /// 记录本场战斗使用过的卡牌
    /// </summary>
    private void RecordUsedCardInThisBattle(Card card)
   {
       thisBattleUsedCards.Add(card);
   }

    /// <summary>
    /// 记录本回合使用过的卡牌
    /// </summary>
    private void RecordUsedCardInThisRound(Card card)
   {
       thisRoundUsedCards.Add(card);
   }

    //清空这回合使用过的卡牌记录
    public void ClearThisRoundCards()
   {
       thisRoundUsedCards.Clear();
   }

    ///TODO:未写功能
    /// <summary>
    /// 裂缝牌流逝后进入垃圾牌堆
    /// </summary>
    public void GoIntoRubbish(Card card)
    {
        
    }
    
    /// <summary>
    /// 将某个队列的卡牌填充到另一个卡牌队列（该队列会清空）中
    /// </summary>
    /// <param name="output">输出队列</param>
    /// <param name="input">被填充队列</param>
    /// Sign:不能直接将一个队列等于另一队列，这是把引用赋值出去了，源头队列会被一起改变
    private void ReplaceAndFill(List<Card> output,List<Card> input)
    {
        input.Clear();

        input.AddRange(output);
    }
    
    /// <summary>
    /// 计算本回合打出的某类卡牌数量
    /// </summary>
    /// <param name="cardType">目标卡牌类型，默认为none，代表着本回合打出牌的总数</param>
    /// <returns></returns>
    public int CalCardNumInThisRound(CardType cardType = CardType.None)
    {
        var total = 0;
        foreach (var card in thisRoundUsedCards)
        {
            if (cardType == CardType.None)
            {
                total++;
            }
            else
            {
                if (card.cardType == cardType)
                {
                    total++;
                }
            }
        }

        return total;
    }

    /// <summary>
    /// 通过传入索引获得对应单张手牌
    /// </summary>
    /// <param name="handIndex"></param>
    /// <returns></returns>
    public Card GetOneHandCard(int handIndex)
    {
        if (handIndex < hand.Count)
        {
            return hand[handIndex];
        }
        
        DebugTool.MyDebugError("通过索引获得手牌时出错，索引越界");
        return null;
    }

    /// <summary>
    /// 判断使用的上一张卡牌是否为目标类型的卡牌
    /// </summary>
    public bool CheckLastCardType(CardType cardType)
    {
        if(lastPlayCard != null)
            return lastPlayCard.cardType == cardType;
        
        return false;
    }
    /// <summary>
    /// 判断使用的当前卡牌是否为目标类型的卡牌
    /// </summary>
    public bool CheckCurCardType(CardType cardType)
    {
        return curPlayerCard.cardType == cardType;
    }
    #endregion
   
}
