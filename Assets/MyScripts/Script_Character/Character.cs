using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Character : MonoBehaviour,ICharacterBehavior
{
    [SerializeField] protected CharacterInfo baseInfo;
    private readonly Dictionary<BuffType, int> buffsNumDic = new Dictionary<BuffType, int>();
    private int immnueNum;
    
    public string Name => baseInfo.characterName;

    protected float maxHealth;
    protected float curHealth;
    protected float curArmor;
    protected float curPower;
    protected float curTenacity;

    protected float atkMulRate = 1;
    protected float dfdMulRate = 1;
    protected float dmgMulRate = 1;
    public const float DefaultMulRateValue = 1;

    /// <summary>
    /// 初始化角色的血量、力量、坚韧等
    /// </summary>
    protected virtual void InitializeAttribute()
    {
        maxHealth = baseInfo.defaultHealth;
        curHealth = maxHealth;

        curPower = baseInfo.defaultPower;
        curTenacity = baseInfo.defaultTenacity;

        immnueNum = 0;

        atkMulRate = DefaultMulRateValue;
        dfdMulRate = DefaultMulRateValue;
        dmgMulRate = DefaultMulRateValue;
    }

    public virtual void Attack(int atkNum, float eachAtkValue, bool withAtkRate = true,params Character[] targets)
    {
        var atkValue = withAtkRate ? Mathf.Round((eachAtkValue + curPower) * atkMulRate) : Mathf.Round((eachAtkValue + curPower));
        
        foreach (var target in targets)
        {
            for (var i = 0; i < atkNum; i++)
            {
                target.TakenDamage(atkValue);
            }
        }

        DebugTool.MyDebug($"{gameObject.name}发动攻击{atkNum}下，每下值为{atkValue}，目标数量为{targets.Length}");
    }

    public virtual void Defend(int dfdNum, float eachDfdValue,bool withDfdRate = true)
    {
        var defdValue = withDfdRate ? Mathf.Round((eachDfdValue + curTenacity) * dfdMulRate) :  Mathf.Round((eachDfdValue + curTenacity));
        
        for (var i = 0; i < dfdNum ; i++)
        {
            curArmor += defdValue;
        }
        
        DebugTool.MyDebug($"{gameObject.name}获得护甲{dfdNum}层，每层值为{defdValue}");
    }

    public virtual void RestoreHealth(float restoreValue,bool isPercentage = false)
    {
        if(curHealth == maxHealth) return;

        var restore = isPercentage ? curHealth * restoreValue : restoreValue;
        
        curHealth = Mathf.Min(curHealth + restore, maxHealth);
        
        DebugTool.MyDebug($"{gameObject.name}恢复血量：{restore}，当前血量为：{curHealth}");
    }

    public virtual void TakenDamage(float dmgValue,bool withDmgRate = true)
    {
        var dmg = withDmgRate ? Mathf.Round(dmgValue * dmgMulRate) : Mathf.Round(dmgValue);
        
        if (curHealth <= dmg)
        {
            DebugTool.MyDebug($"{gameObject.name}受到伤害:{dmg}，伤害溢出");
            Death();
            return;
        }

        curHealth = Mathf.Max(curHealth - dmg, 0);
        
        DebugTool.MyDebug($"{gameObject.name}受到伤害:{dmg}，当前生命值为{curHealth}");
    }
    
    public virtual void GiveBuff(CharacterBuff buff,int buffNum,params Character[] targets)
    {
        foreach (var target in targets)
        {
            target.GetBuff(buff,buffNum);
            
            DebugTool.MyDebug($"{gameObject.name}给予{target.gameObject.name}{buffNum}层buff{buff.buffType}");
        }
    }

    public virtual void Death()
    {
        DebugTool.MyDebug("该角色死亡" + baseInfo.characterName);
    }

    /// <summary>
    /// 判断生命值是否低于N%
    /// </summary>
    /// <param name="percentage">目标百分数比</param>
    /// <returns></returns>
    public bool CheckHealthLessThanPercentage(float percentage)
    {
        return curHealth < maxHealth * percentage;
    }
    
    /// <summary>
    /// 获得/减少力量
    /// </summary>
    public void GetPower(float powerValue)
    {
        curPower -= powerValue;
    }
    /// <summary>
    /// 获得/减少坚韧
    /// </summary>
    public void GetTenacity(float tenacityValue)
    {
        curTenacity -= tenacityValue;
    }
    
    /// <summary>
    /// 获得Buff
    /// </summary>
    /// <param name="buffNum">Buff层数</param>
    public void GetBuff(CharacterBuff characterBuff,int buffNum)
    {
        //获得buff时立刻生效
        BuffProvider.Instance.BuffEffect(characterBuff,this);

        if (characterBuff.buffType == BuffType.Immune)
        {
            immnueNum++;
        }
        else
        {
            if (!buffsNumDic.ContainsKey(characterBuff.buffType))
            {
                buffsNumDic.Add(characterBuff.buffType,buffNum);
            }
            else
            {
                var remainingNum = 0;
                
                if (immnueNum > 0)
                {
                    if (immnueNum >= buffNum)
                    {
                        immnueNum = Mathf.Max(immnueNum - buffNum, 0);
                    }
                    else
                    {
                        remainingNum = buffNum - immnueNum;
                        immnueNum = 0;
                    }
                }
               
                if (remainingNum <= 0) return;

                buffsNumDic[characterBuff.buffType] = remainingNum;
            }
        }
    }
    
    /// <summary>
    /// Buff衰减1层
    /// </summary>
    public void BuffDecrease()
    {
        if(buffsNumDic.Count == 0) return;

        //TODO:SIGH:foreach内字典不能更改值,将key取出后再通过list修改对应的value
        var list = buffsNumDic.Keys.ToList();

        for (var i = 0; i < list.Count; i++)
        {
            if(buffsNumDic[list[i]] == 0 ) continue;

            buffsNumDic[list[i]] -= 1;
            
            //DebugTool.MyDebug($"buff:{list[i].Item2}减少1层，减少后当前层数为{buffsNumDic[list[i]]}");
            
            if(buffsNumDic[list[i]] == 0) 
                BuffProvider.Instance.BuffEnd(list[i],this);
        }
    }

    /// <summary>
    /// 清空身上的buff（TODO：应是Debuff或BUff）
    /// </summary>
    public void ClearBuff()
    {
        buffsNumDic.Clear();
    }

    /// <summary>
    /// 角色获得1层免疫效果
    /// </summary>
    public void GetImmune()
    {
        immnueNum++;
    }

    /// <summary>
    /// 改变攻击倍率,默认为1（代表复原）
    /// </summary>
    public void ChangeAtkMulRate(float newAtkMulRate = 1)
    {
        atkMulRate = newAtkMulRate;
    }
    /// <summary>
    /// 改变防御倍率,默认为1（代表复原）
    /// </summary>
    public void ChangeDfdMulRate(float newDfdMulRate = 1)
    {
        dfdMulRate = newDfdMulRate;
    }
    /// <summary>
    /// 改变受伤倍率,默认为1（代表复原）
    /// </summary>
    public void ChangeDmgMulRate(float newDmgMulRate = 1)
    {
        dmgMulRate = newDmgMulRate;
    }

    /// <summary>
    /// 角色受到晕眩效果
    /// </summary>
    public virtual void BeFaintness()
    {
        
    }

    /// <summary>
    /// 角色受到失明效果
    /// </summary>
    public virtual void BeBlind()
    {
        
    }

    /// <summary>
    /// 角色隐匿效果
    /// </summary>
    public virtual void BeStealth()
    {
        
    }
}