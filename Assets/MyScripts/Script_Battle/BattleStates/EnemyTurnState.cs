using UnityEngine;

[CreateAssetMenu(menuName = "BattleState/NewEnemyTurnState",fileName = "EnemyTurnState")]
public class EnemyTurnState : BattleStateBase
{
    public override void OnEnter()
    {
        base.OnEnter();
        
        BattleManager.Instance.LaunchEnemyBehavior();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        
        if(!BattleManager.Instance.InBattle)
            battleStateMachine.SwitchState(BattleStateEnum.BattleOver);
        
        if(BattleManager.Instance.EnemyBehaviorsClear)
            battleStateMachine.SwitchState(BattleStateEnum.EnemyTurnOver);
    }

    public override void OnExit()
    {
        base.OnExit();
        
    }
}