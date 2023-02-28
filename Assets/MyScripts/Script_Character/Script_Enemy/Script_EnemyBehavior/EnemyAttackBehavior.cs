using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

//TODO：现将随机范围获得的伤害四舍五入了
[CreateAssetMenu(menuName = "EnemyBehavior/NewEnemyAttackBehavior",fileName = "NewEnemyAttackBehavior")]
public class EnemyAttackBehavior : EnemyBehavior
{
    /// <summary>
    /// 若不是随机范围攻击则为当前敌人攻击数值
    /// </summary>
    [SerializeField] private float minAtkValue;
    [SerializeField] private float maxAtkValue;
    public int atkNum = 1;
    public bool isRandomAtk;

    [Header("Buff攻击")]
    [SerializeField] private List<CharacterBuff> buffList = new List<CharacterBuff>();
    [SerializeField] private List<int> buffNumList = new List<int>();
    public readonly Dictionary<CharacterBuff, int> BuffsNumsDic = new Dictionary<CharacterBuff, int>();

    private void OnEnable()
    {
        if(buffNumList.Count == 0) return;

        if (buffNumList.Count == buffNumList.Count)
        {
            for (var i = 0; i < buffList.Count; i++)
            {
                BuffsNumsDic.Add(buffList[i],buffNumList[i]);
            }
        }
        else
        {
            DebugTool.MyDebugError($"敌人攻击行为buff和buff层数初始化时出错，两者数量不相等");
        }
    }

    /// <summary>
    /// 获得攻击值，若不是随机范围攻击则为当前敌人攻击数值
    /// </summary>
    public float GetAtKValue()
    {
        var atkValue = 0f;

        atkValue = isRandomAtk ? atkValue = Random.Range(minAtkValue, maxAtkValue + 1) : minAtkValue;

        return Mathf.Round(atkValue);
    }
}


