using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO：模板实例化复制使用，作为模板再包括好像反而不太合适
//TODO: 感觉方法调用没写好，变成了我在你的内部调用我的方法...
[CreateAssetMenu(menuName = "CharacterBuff/NewCharacterBuff",fileName = "NewCharacterBuff")]
public class CharacterBuff : ScriptableObject
{
    public BuffType buffType;
    /// <summary>
    /// buff涉及到的值，0则代表无数值相关
    /// </summary>
    public float buffValue = 0;
}

public enum BuffType
{
    EasilyHurt,
    Flinch,
    ArmorBroken,
    Faintness,
    Blind,
    Stealth,
    Immune,
}
