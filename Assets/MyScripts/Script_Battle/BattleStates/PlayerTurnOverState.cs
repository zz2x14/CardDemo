using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BattleState/NewPlayerTurnOverState",fileName = "PlayerTurnOverState")]
public class PlayerTurnOverState : BattleStateBase
{
    public override void OnEnter()
    {
        base.OnEnter();
        
        BattleManager.Instance.PlayerTurnOver();
        
        battleStateMachine.SwitchState(BattleStateEnum.EnemyTurnBegin);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        
        if(!BattleManager.Instance.InBattle)
            battleStateMachine.SwitchState(BattleStateEnum.BattleOver);
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}