using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "EnemyCombination/NewEnemyCombinationLibrary",fileName = "NewEnemyCombinationLibrary")]
public class EnemyCombinationLibrary : ScriptableObject
{
    [SerializeField] private List<EnemyCombination> combinationList = new List<EnemyCombination>();

    private readonly Dictionary<List<EnemyInfo>,List<int>> enemyBebaviorIndexDic = new Dictionary<List<EnemyInfo>, List<int>>();
    
    /// <summary>
    /// 根据传入的目标敌人等级随机返回等级匹配的敌人们的信息
    /// </summary>
    public List<EnemyInfo> RandomlySpawnEnemiesWithRuler(EnemyCombinationLevel level)
    {
        var levelCombinations = combinationList.Where(combination => combination.enemyCombinationLevel == level).ToList();

        var randomNum = Random.Range(0, levelCombinations.Count);

        return levelCombinations[randomNum].enemyInfoList;
    }

    /// <summary>
    /// 根据传入的敌人信息队列获得对应的敌人行为索引队列
    /// </summary>
    public List<int> GetThisBattleEnemiesBehaviorIndexs(List<EnemyInfo> enemyInfos)
    {
        if (enemyBebaviorIndexDic.ContainsKey(enemyInfos))
        {
            return enemyBebaviorIndexDic[enemyInfos];
        }
        else
        {
            Debug.LogError("获取敌人行为索引时出错，字典中未拥有传入的敌人信息队列");
            return null;
        }
    }

    private void OnEnable()
    {
        foreach (var combination in combinationList)
        {
            if (combination.Check())
            {
                enemyBebaviorIndexDic.Add(combination.enemyInfoList,combination.enemyBehaviorIndexList);
            }
            else
            {
                Debug.Log("敌人组合检查为对等");
            }
        }
    }
}

[Serializable]
public class EnemyCombination 
{
    public List<EnemyInfo> enemyInfoList = new List<EnemyInfo>();
    public List<int> enemyBehaviorIndexList = new List<int>();
    public EnemyCombinationLevel enemyCombinationLevel;

    /// <summary>
    /// 检查敌人数量和敌人行为序号数量是否对等
    /// </summary>
    public bool Check()
    {
        if (enemyInfoList.Count == enemyBehaviorIndexList.Count) return true;
        
        Debug.LogError("敌人数量和敌人行为序号数量不对等，组合内敌人名字如下：");
        foreach (var enemyInfo in enemyInfoList)
        {
            Debug.Log(enemyInfo.characterName);
        }

        return false;
    }
}

public enum EnemyCombinationLevel
{
    C,
    B,
    A
}
 