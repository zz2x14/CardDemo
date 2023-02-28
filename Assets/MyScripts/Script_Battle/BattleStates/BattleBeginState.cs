using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;

[CreateAssetMenu(menuName = "BattleState/NewBattleBeginState",fileName = "BattleBeginState")]
public class BattleBeginState : BattleStateBase
{
    public override void OnEnter()
    {
        base.OnEnter();
        
        BattleManager.Instance.BattleBegin();
        
        battleStateMachine.SwitchState(BattleStateEnum.PlayerTurnBegin);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        
       //TODO:战斗开始前还需要作一些检查操作 比如某些在战斗开始时触发的效果等
       
       
       if(!BattleManager.Instance.InBattle)
           battleStateMachine.SwitchState(BattleStateEnum.BattleOver);
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}
