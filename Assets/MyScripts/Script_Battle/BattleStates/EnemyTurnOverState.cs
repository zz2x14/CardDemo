using UnityEngine;

[CreateAssetMenu(menuName = "BattleState/NewEnemyTurnOverState",fileName = "EnemyTurnOverState")]
public class EnemyTurnOverState : BattleStateBase
{
    public override void OnEnter()
    {
        base.OnEnter();

        battleStateMachine.SwitchState(BattleStateEnum.PlayerTurnBegin);
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
        
        BattleManager.Instance.EnemyTurnOver();
    }
}