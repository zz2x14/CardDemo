using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BattleState/NewBattleOverState",fileName = "BattleOverState")]
public class BattleOverState : BattleStateBase
{
    public override void OnEnter()
    {
        base.OnEnter();
        
        BattleManager.Instance.ClearBattleEnemyList();
        MapManager.Instance.ClearEnemyContainers();
        
        battleStateMachine.SwitchState(BattleStateEnum.NoBattle);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    public override void OnExit()
    {
        base.OnExit();
        
        BattleManager.Instance.BattleOver();
    }
}