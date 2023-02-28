using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BattleState/NewPlayerTurnState",fileName = "PlayerTurnState")]
public class PlayerTurnState : BattleStateBase
{
    public override void OnEnter()
    {
        base.OnEnter();
        
        BattleManager.Instance.InPlayerTurn = true;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        
        if(!BattleManager.Instance.InBattle)
            battleStateMachine.SwitchState(BattleStateEnum.BattleOver);
        
        if(!BattleManager.Instance.InPlayerTurn)
            battleStateMachine.SwitchState(BattleStateEnum.PlayerTurnOver);
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}