using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPlayerManager : PersistentSingleton<MainPlayerManager>
{
    [Header("当前使用角色")] 
    public MainPlayer CurMainPlayer;
    
    /// <summary>
    /// 玩家角色恢复血量
    /// </summary>
    /// <param name="restoreValue">恢复量值（百分百或固定值）</param>
    /// <param name="isPercentage">是否为百分比恢复，默认为false</param>
    public void MainPlayerRestoreHealth(float restoreValue, bool isPercentage = false)
    {
        CurMainPlayer.RestoreHealth(restoreValue,isPercentage);
    }
}
