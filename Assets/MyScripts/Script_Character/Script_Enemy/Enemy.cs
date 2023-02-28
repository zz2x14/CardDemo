using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敌人容器，详细是什么敌人由其装载得EnemyInfo决定
/// </summary>
public class Enemy : Character
{
    private EnemyInfo enemyInfo;
    private EnemyContainer enemyContainer;
    public Sprite EnemyImage => enemyInfo.characterImage;

    /// <summary>
    /// 敌人在战斗中的站位编号
    /// </summary>
    public int EnemyBattlePos { get; set; }

    public EnemyBehavior CurBehavior { get; private set; }
    private EnemyBehavior nextBehavior;

    private List<EnemyBehavior> thisBattleBehaviors = new List<EnemyBehavior>();

    private List<Character> behaviorTargets = new List<Character>();

    private void Awake()
    {
        enemyContainer = GetComponent<EnemyContainer>();
    }

    /// <summary>
    /// 当前敌人容器是否已经填充了敌人
    /// </summary>
    public bool EnemyExisting()
    {
        return enemyInfo != null;
    }

    /// <summary>
    /// 改变该容器的敌人信息
    /// </summary>
    public void GetInfo(EnemyInfo enemyInfo)
    {
        ClearEnemyInfo();
        
        
        this.enemyInfo = Instantiate(enemyInfo);
        baseInfo = Instantiate(enemyInfo);

        if(enemyInfo == null) return;
        
        InitializeAttribute();
    }
    
    /// <summary>
    /// 重置（清空）当前敌人信息
    /// </summary>
    public void ClearEnemyInfo()
    {
        baseInfo = null;
        enemyInfo = null;

        CurBehavior = null;
        nextBehavior = null;

        EnemyBattlePos = -1;

        behaviorTargets.Clear();
        
        thisBattleBehaviors.Clear();
    }

    /// <summary>
    /// 获得该敌人对应的战斗行为列表
    /// </summary>
    /// <param name="behaviorIndex">传入行为索引</param>
    public void GetCurBattleEnemyBehaviors(int behaviorIndex)
    {
        if (behaviorIndex == 0)
        {
            Debug.LogError("传入的behaviorIndex错误，不能为0");
            return;
        }

        if (behaviorIndex > enemyInfo.enemyBehaviorContainers.Count)
        {
            Debug.LogError("传入的behaviorIndex超出行为队列长度");
            return;
        }
        
        //Debug.Log("获得行为索引：" + behaviorIndex);

        var list = new List<EnemyBehavior>();
        
        foreach (var behavior in enemyInfo.enemyBehaviorContainers[behaviorIndex - 1].enemyBehaviorList)
        {
            list.Add(Instantiate(behavior));
        }

        thisBattleBehaviors = list;
    }

    /// <summary>
    /// 寻找当前行为目标对象
    /// </summary>
    public Character[] FindTargets()
    {
        behaviorTargets.Clear();

        switch (CurBehavior.enemyBehaviorRange)
        {
            case EnemyBehaviorRange.ForSelf:
                behaviorTargets.Add(this);
                break;
            case EnemyBehaviorRange.ForPlayer:
                behaviorTargets.Add(MainPlayerManager.Instance.CurMainPlayer);
                break;
            case EnemyBehaviorRange.ForPointedFellow:
                break;
            case EnemyBehaviorRange.ForRandomFellow:
                break;
            case EnemyBehaviorRange.ForAllFellow:
                behaviorTargets.AddRange(BattleManager.Instance.CurBattleEnemyList);
                break;
            case EnemyBehaviorRange.ForAllCharacter:
                behaviorTargets.Add(MainPlayerManager.Instance.CurMainPlayer);
                behaviorTargets.AddRange(BattleManager.Instance.CurBattleEnemyList);
                break;
        }

        return behaviorTargets.ToArray();
    }

    /// <summary>
    /// 进入战斗时敌人执行
    /// </summary>
    public void ReadyForBattle()
    {
        if (thisBattleBehaviors.Count > 0)
        {
            CurBehavior = thisBattleBehaviors[0];
        }

        nextBehavior = thisBattleBehaviors.Count > 1 ? thisBattleBehaviors[1] : CurBehavior;

        //Debug.Log($"载体{gameObject.name}的预备行为{CurBehavior}");
        //Debug.Log($"载体{gameObject.name}的下个行为{nextBehavior}");
    }

    /// <summary>
    /// 执行当前行为
    /// </summary>
    public void BehaviorWork()
    {
        if(CurBehavior == null) return;

        //Debug.Log($"载体{gameObject.name}采取行动{CurBehavior}");
        
        switch (CurBehavior.enemyBehaviorType)
        {
            case EnemyBehaviorType.Nothing:
                break;
            case EnemyBehaviorType.Attack:
                var atk = CurBehavior as EnemyAttackBehavior;
                Attack(atk.atkNum,atk.GetAtKValue(),true,FindTargets());
                if (atk.BuffsNumsDic.Count != 0)
                {
                    foreach (var buff in atk.BuffsNumsDic.Keys)
                    {
                        GiveBuff(buff,atk.BuffsNumsDic[buff] != 0 ? atk.BuffsNumsDic[buff] : atk.atkNum,FindTargets());
                    }
                }
                break;
            case EnemyBehaviorType.Defend:
                var dfd = CurBehavior as EnemyDefendBehavior;
                Defend(dfd.dfdNum,dfd.dfdValue);
                break;
            case EnemyBehaviorType.Buff:
                break;
            case EnemyBehaviorType.Escape:
                break;
        }
        
        BattleManager.Instance.RemoveBehaviorFromCurRoundList(this,CurBehavior); 
        SwitchBehavior();
    }

    //改变行为
    private void SwitchBehavior()
    {
        if (CurBehavior == null || nextBehavior == null)
        {
            Debug.LogError($"敌人{Name}当前行为或下一行为空");
            return;
        }

        var curIndex = thisBattleBehaviors.IndexOf(CurBehavior);
        var nextIndex = 0;

        if (curIndex < thisBattleBehaviors.Count - 1)
        {
            nextIndex = curIndex + 1;
        }
        else if (curIndex == thisBattleBehaviors.Count - 1)
        {
            nextIndex = 0;
        }

        CurBehavior = nextBehavior;
        nextBehavior = thisBattleBehaviors[nextIndex];
    }
    
    public override void Attack(int atkNum, float eachAtkValue, bool withAtkRate = true, params Character[] targets)
    {
        base.Attack(atkNum,eachAtkValue,withAtkRate,targets);
    }

    public override void Defend(int dfdNum, float eachDfdValue , bool withDfdRate = true)
    { 
        base.Defend(dfdNum,eachDfdValue,withDfdRate);
    }

    public override void Death()
    {
        base.Death();
        
        BattleManager.Instance.RemoveBattleEnemy(this);
        
        enemyContainer.Clear();
        ClearEnemyInfo();
    }
}
