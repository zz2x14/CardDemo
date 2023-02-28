using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CharacterAttribute/NewEnemyAttribute",fileName = "NewEnemyAttribute")]
public class EnemyInfo : CharacterInfo
{
    [Header("敌人私有属性")] 
    public int enemyId;
    [Header("敌人行动列表")] 
    public List<EnemyBehaviorContainer> enemyBehaviorContainers = new List<EnemyBehaviorContainer>();
}

[System.Serializable]
public class EnemyBehaviorContainer
{
    public List<EnemyBehavior> enemyBehaviorList = new List<EnemyBehavior>();
}

