using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Card/BattleCard/NewDefendCard",fileName = "NewDefendCard")]
public class DefenceCard : BattleCard
{
    [Header("防御卡牌属性")]
    [SerializeField] protected float dfdValue;
    [SerializeField] protected int dfdNum = 1;

    protected override Character[] CardTargets()
    {
        return base.CardTargets();
    }

    protected override void CardWork(params Character[] targets)
    {
        base.CardWork(targets);
        
        //Debug.Log("使用防御卡牌");
        
        owner.Defend(dfdNum,dfdValue);
    }
}
