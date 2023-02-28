using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Card/BattleCard/NewAttackCard",fileName = "NewAttackCard")]
public class AttackCard : BattleCard
{
    [Header("攻击卡牌属性")]
    [SerializeField] private float atkValue;
    [SerializeField] private int atkNum = 1;

    protected override Character[] CardTargets()
    {
        return BattleManager.Instance.GetAtkTargetEnemies(this).ToArray();
    }

    protected override void CardWork(params Character[] targets)
    {
        base.CardWork(targets);
        
        if (targets.Length <= 0) return;
        //Debug.Log("使用攻击卡牌");
        
        owner.Attack(atkNum,atkValue,true,targets);

        if (buffsNumsDic.Count == 0) return;
        
        foreach (var buff in buffsNumsDic.Keys)
        {
            owner.GiveBuff(buff,buffsNumsDic[buff] != 0 ? buffsNumsDic[buff] : atkNum,targets);
        }
    }

  
}

