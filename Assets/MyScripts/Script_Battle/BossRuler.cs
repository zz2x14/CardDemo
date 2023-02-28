using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ruler/NewBossRuler",fileName = "NewBossRuler")]
public class BossRuler : ScriptableObject
{
    [Range(0, 100)] public int bossIncreasedRate_Enemy;
    [Range(0, 100)] public int bossIncreasedRate_EliteEnemy;
    [Range(0, 100)] public int bossIncreasedRate_NoBattle;
    public int spaceStoneGet_Enemy;
    public int spaceStoneGet_EliteEnemy;
}
