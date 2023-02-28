using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// 提供UI快捷方法
/// </summary>
public static class UITool
{
    public static void BtnAddListener(Button btn,UnityAction func)
    {
        btn.onClick.AddListener(func);
    }
    public static void BtnRemoveOneListener(Button btn, UnityAction func)
    {
        btn.onClick.RemoveListener(func);
    }
    public static void BtnRemoveAllListeners(Button btn)
    {
        btn.onClick.RemoveAllListeners();
    }
}
