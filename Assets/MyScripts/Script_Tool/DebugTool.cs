using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Debug工具类
/// </summary>
public static class DebugTool 
{
    /// <summary>
    /// Unity_Debug
    /// </summary>
    /// <param name="content">内容</param>
    /// <param name="doDebug">是否执行打印，默认为true</param>
    public static void MyDebug(string content,bool doDebug = true)
    {
        if (doDebug)
        {
            Debug.Log(content);
        }
    }
    
    /// <summary>
    /// Unity_DebugWarning
    /// </summary>
    /// <param name="content">内容</param>
    /// <param name="doDebug">是否执行打印，默认为true</param>
    public static void MyDebugWarning(string content,bool doDebug = true)
    {
        if (doDebug)
        {
            Debug.LogWarning(content);
        }
    }
    
    /// <summary>
    /// Unity_DebugError
    /// </summary>
    /// <param name="content">内容</param>
    /// <param name="doDebug">是否执行打印，默认为true</param>
    public static void MyDebugError(string content,bool doDebug = true)
    {
        if (doDebug)
        {
            Debug.LogError(content);
        }
    }
}
