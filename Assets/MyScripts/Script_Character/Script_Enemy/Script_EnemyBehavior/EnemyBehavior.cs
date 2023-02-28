using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敌人行为SO基类
/// </summary>
public abstract class EnemyBehavior : ScriptableObject
{
    public EnemyBehaviorType enemyBehaviorType;
    public EnemyBehaviorRange enemyBehaviorRange;
    
    
}

public enum EnemyBehaviorType
{
    Nothing,
    Attack,
    Defend,
    Buff,
    Escape
}

public enum EnemyBehaviorRange
{
    ForSelf,
    ForPlayer,
    ForPointedFellow,
    ForRandomFellow,
    ForAllFellow,
    ForAllCharacter,
}
