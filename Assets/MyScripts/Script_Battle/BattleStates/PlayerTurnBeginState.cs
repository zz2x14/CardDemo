using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BattleState/NewPlayerTurnBeginState",fileName = "PlayerTurnBeginState")]
public class PlayerTurnBeginState : BattleStateBase
{
    public override void OnEnter()
    {
        base.OnEnter();
        
        BattleManager.Instance.CurRoundNum++;
        
        BattleManager.Instance.PlayerTurnBegin();
        
        battleStateMachine.SwitchState(BattleStateEnum.PlayerTurn);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        
        //TODO:玩家回合开始前还需要作一些检查操作 比如某些在回合开始时触发的效果等

        if(!BattleManager.Instance.InBattle)
            battleStateMachine.SwitchState(BattleStateEnum.BattleOver);
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}