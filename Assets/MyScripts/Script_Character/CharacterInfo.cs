using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// 仅用来配置角色属性
/// </summary>
public class CharacterInfo : ScriptableObject
{
    [Header("公有属性")] 
    public string characterName;
    public float defaultHealth;
    public int defaultPower;
    public int defaultTenacity;

    [Header("图像")] 
    public Sprite characterImage;
}
