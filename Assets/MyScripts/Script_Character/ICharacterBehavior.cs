using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacterBehavior
{
    public void Attack(int atkNum, float eachAtkValue, bool withAtkRate = true,params Character[] targets);

    public void Defend(int dfdNum, float eachDfdValue,bool withDfdRate = true);

    /// <summary>
    /// 恢复血量
    /// </summary>
    /// <param name="restoreValue">恢复量值（百分百或固定值）</param>
    /// <param name="isPercentage">是否为百分比恢复，默认为false</param>
    public void RestoreHealth(float restoreValue,bool isPercentage);

    public void TakenDamage(float dmgValue,bool withDmgRate = true);

    public void Death();

    public void GiveBuff(CharacterBuff buff,int buffNum,params Character[] target);
}
