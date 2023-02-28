using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStateMachine : MonoBehaviour
{
    private BattleStateBase curState;

    [SerializeField] private List<BattleStateBase> battleStateList = new List<BattleStateBase>();

    private readonly Dictionary<BattleStateEnum, BattleStateBase> battleStateDic = new Dictionary<BattleStateEnum, BattleStateBase>();

    private void Awake()
    {
        InitializeStates();
    }
    
    private void Update()
    {
        if(curState != null)
            curState.OnUpdate();
    }

    private void InitializeStates()
    {
        foreach (var state in battleStateList)
        {
            state.Initialize(this);
        }
    }

    /// <summary>
    /// 加入状态机字典
    /// </summary>
    public void BindDic(BattleStateBase state)
    {
        if (battleStateDic.ContainsKey(state.battleStateEnum)) return; 
        
        battleStateDic.Add(state.battleStateEnum,state);
    }

    /// <summary>
    /// 切换状态
    /// </summary>
    public void SwitchState(BattleStateEnum stateEnum)
    {
        if (!battleStateDic.ContainsKey(stateEnum))
        {
            Debug.LogError("目标BattleState状态不存在：" + stateEnum);
            return;
        }
        
        if(battleStateDic[stateEnum] == curState) return;
        
        if(curState!=null)
            curState.OnExit();

        curState = battleStateDic[stateEnum];
        
        curState.OnEnter();
    }
    
   
    
    /// <summary>
    /// 启动状态机 默认未进入战斗状态
    /// </summary>
    public void SwitchOn()
    {
        SwitchState(BattleStateEnum.NoBattle);
    }

    /// <summary>
    /// 进入玩家回合结束阶段
    /// </summary>
    public void GoPlayerTurnOverState()
    {
        SwitchState(BattleStateEnum.PlayerTurnOver);
    }
    
    /// <summary>
    /// 战斗结束 退出战斗 - 将状态滞空
    /// </summary>
    public void ExitBattle()
    {
        curState = null;
    }

   
}
