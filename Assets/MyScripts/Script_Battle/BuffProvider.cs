using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 公共提供buff模板，且提供Buff相关功能
/// </summary>
public class BuffProvider : PersistentSingleton<BuffProvider>
{
    public CharacterBuff EasyHurt;
    public CharacterBuff Flinch;
    public CharacterBuff ArmorBroken;
    public CharacterBuff Faintness;
    public CharacterBuff Blind;
    public CharacterBuff Imnnue;
    
    /// <summary>
    /// 对应Buff效果
    /// </summary>
    public void BuffEffect(CharacterBuff buff,Character target)
    {
        switch (buff.buffType)
        {
            case BuffType.EasilyHurt:
                target.ChangeDmgMulRate(buff.buffValue);
                break;
            case BuffType.Flinch:
                target.ChangeAtkMulRate(buff.buffValue);
                break;
            case BuffType.ArmorBroken:
                target.ChangeDfdMulRate(buff.buffValue);
                break;
            case BuffType.Faintness:
                target.BeFaintness();
                break;
            case BuffType.Blind:
                target.BeBlind();
                break;
            case BuffType.Stealth:
                target.BeStealth();
                break;
            case BuffType.Immune:
                target.GetImmune();
                break;
        }
    }

    /// <summary>
    /// Buff结束
    /// </summary>
    public void BuffEnd(BuffType buffType,Character target)
    {
        switch (buffType)
        {
            case BuffType.EasilyHurt:
                target.ChangeDmgMulRate();
                break;
            case BuffType.Flinch:
                target.ChangeAtkMulRate();
                break;
            case BuffType.ArmorBroken:
                target.ChangeDfdMulRate();
                break;
            case BuffType.Faintness:
                target.BeFaintness();
                break;
            case BuffType.Blind:
                target.BeBlind();
                break;
            case BuffType.Stealth:
                target.BeStealth();
                break;
        }
    }
}
