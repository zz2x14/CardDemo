using UnityEngine;

[CreateAssetMenu(menuName = "BattleState/NewEnemyTurnBeginState",fileName = "EnemyTurnBeginState")]
public class EnemyTurnBeginState : BattleStateBase
{
    public override void OnEnter()
    {
        base.OnEnter();
        
        BattleManager.Instance.EnemyTurnBegin();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        
        if(!BattleManager.Instance.InBattle)
            battleStateMachine.SwitchState(BattleStateEnum.BattleOver);
        
        if(BattleManager.Instance.EnemyTurnBeginReady)
            battleStateMachine.SwitchState(BattleStateEnum.EnemyTurn);
    }

    public override void OnExit()
    {
        base.OnExit();

        BattleManager.Instance.EnemyTurnBeginReady = false;
    }
}