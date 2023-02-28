using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PersistentSingleton<T> : MonoBehaviour where T :PersistentSingleton<T>
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = (T) this;
        }
        else
        {
            if (Instance == (T) this) return;
            Debug.LogError($"场景中已经存在一个：{gameObject.name}");
            Destroy(gameObject);
        }
        
        DontDestroyOnLoad(gameObject);
    }
}
