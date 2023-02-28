using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class BattleStateBase : ScriptableObject
{
    public BattleStateEnum battleStateEnum;
    
    protected BattleStateMachine battleStateMachine;
        
    public void Initialize(BattleStateMachine stateMachine)
    {
        battleStateMachine = stateMachine;
        
        battleStateMachine.BindDic(this);
    }

    public virtual void OnEnter()
    {
        BattleManager.Instance.ChangeBattleState(battleStateEnum);
        
        DebugTool.MyDebug(name,BattleManager.Instance.doDebugState);
    }

    public virtual void OnUpdate()
    {
        
    }

    public virtual void OnExit()
    {
        
    }
}
