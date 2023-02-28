using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : PersistentSingleton<PoolManager>
{
    private readonly Dictionary<GameObject, Pool> poolDic = new Dictionary<GameObject, Pool>();
    private readonly Dictionary<GameObject, Transform> parentTfDic = new Dictionary<GameObject, Transform>();

    [SerializeField] private Pool[] UIPools;

    protected override void Awake()
    {
        base.Awake();

        InitializePools(UIPools);
    }

    private void InitializePools(Pool[] pools)
    {
        for (int i = 0; i < pools.Length; i++)
        {
            var poolParent = new GameObject($"Pool:{pools[i].Prefab.name}");
            poolParent.transform.parent = transform;
            pools[i].Initialize(poolParent.transform);
            
            poolDic.Add(pools[i].Prefab,pools[i]);
        }
    }

    public GameObject Release(GameObject go)
    {
        if (poolDic.ContainsKey(go))
        {
            return poolDic[go].GetPreparedGO();
        }
        
        Debug.LogError($"{go.name}并不存在于对象池字典中");
        return null;
    }
    public GameObject Release(GameObject go,Vector3 pos)
    {
        if (poolDic.ContainsKey(go))
        {
            return poolDic[go].GetPreparedGO(pos);
        }
        
        Debug.LogError($"{go.name}并不存在于对象池字典中");
        return null;
    }
    public GameObject Release(GameObject go,Vector3 pos,Quaternion rotation)
    {
        if (poolDic.ContainsKey(go))
        {
            return poolDic[go].GetPreparedGO(pos,rotation);
        }
        
        Debug.LogError($"{go.name}并不存在于对象池字典中");
        return null;
    }
    public GameObject Release(GameObject go,Vector3 pos,Quaternion rotation,Vector3 scale)
    {
        if (poolDic.ContainsKey(go))
        {
            return poolDic[go].GetPreparedGO(pos,rotation,scale);
        }
        
        Debug.LogError($"{go.name}并不存在于对象池字典中");
        return null;
    }

    /// <summary>
    /// 将从对象池生成出的对象回到原本的父对象位置下
    /// </summary>
    public void ReturnDefaultParentTransform(GameObject go)
    {
        if (parentTfDic.ContainsKey(go))
        {
            go.gameObject.SetActive(false);
            go.transform.SetParent(parentTfDic[go]);
        }
        else
        {
            DebugTool.MyDebugError($"将游戏物体{go.name}归还到对象池的原本父物体位置时出错，并未在字典中");
        }
    }

    /// <summary>
    /// 将后续生成的预制体对象进行初始位置字典绑定
    /// </summary>
    public void BindDefaultParentTf(GameObject go,Transform parent)
    {
        if (!parentTfDic.ContainsKey(go))
        {
            parentTfDic.Add(go,parent);
        }
        else
        {
            DebugTool.MyDebugError($"游戏物体{go.name}已经在位置字典中");
        }
    }
    
}
