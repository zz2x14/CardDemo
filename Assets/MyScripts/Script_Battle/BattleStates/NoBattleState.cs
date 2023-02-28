using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BattleState/NewNoBattleState",fileName = "NoBattleState")]
public class NoBattleState : BattleStateBase
{
    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        
        if(BattleManager.Instance.InBattle)
            battleStateMachine.SwitchState(BattleStateEnum.BattleBegin);
        
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}
