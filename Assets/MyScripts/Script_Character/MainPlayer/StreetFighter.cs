using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class StreetFighter : MainPlayer
{
   [SerializeField] private StreetFightWeapon curWeapon;
   [SerializeField] private List<StreetFightWeapon> weaponLibrary = new List<StreetFightWeapon>();
   private readonly Dictionary<WeaponType, StreetFightWeapon> weaponDic = new Dictionary<WeaponType, StreetFightWeapon>();
   
   protected override void Awake()
   {
       base.Awake();
       
       InitializeAttribute();
       
       InitializeWeapon();
   }

   private void InitializeWeapon()
   {
       foreach (var weapon in weaponLibrary)
       {
           weaponDic.Add(weapon.weaponType,GameObject.Instantiate(weapon));
       }

       curWeapon = weaponDic[WeaponType.GolfStick];
   }

   public override void PlayHand(Card card)
   {
       base.PlayHand(card);
       
       CardManager.Instance.UseHandProcess(card);
       
       WeaponWork();
   }

   public override void Attack(int atkNum, float eachAtkValue, bool withAtkRate = true,params Character[] targets)
   {
       StartCoroutine(AttackCor(atkNum, eachAtkValue, withAtkRate, targets));
   }
   //攻击协程
   IEnumerator AttackCor(int atkNum, float eachAtkValue, bool withAtkRate = true,params Character[] targets)
   {
       var atkValue = withAtkRate ? Mathf.Round((eachAtkValue + curPower) * atkMulRate) : Mathf.Round((eachAtkValue + curPower));
       
       DebugTool.MyDebug($"{gameObject.name}发动攻击{atkNum}下，每下值为{atkValue}，目标数量为{targets.Length}");
       
       foreach (var target in targets)
       {
           for (var i = 0; i < atkNum; i++)
           {
               target.TakenDamage(atkValue);

               //等待一段时间后进行追加武器点数伤害
               yield return new WaitForSeconds(0.5f);

               if (curWeapon.weaponType is not WeaponType.GolfStick and WeaponType.GolfStickPlus)
               {
                   if (curWeapon.weaponType is WeaponType.Catapult or WeaponType.CatapultPlus)
                   {
                       for (var j = 0; j < curWeapon.weaponValue01; j++)
                       {
                           target.TakenDamage(curWeapon.weaponAttackPoint,false);
                       
                           yield return new WaitForSeconds(0.5f);
                       }
                   }
                   else
                   {
                       target.TakenDamage(curWeapon.weaponAttackPoint,false);
                   }
               }
               else
               {
                   target.TakenDamage(target.CheckHealthLessThanPercentage(curWeapon.weaponValue01) ? 
                       curWeapon.weaponAttackPoint * curWeapon.weaponValue02 : curWeapon.weaponAttackPoint,false);
                       
                   yield return new WaitForSeconds(0.5f);
               }
           }
       }
       
       StopAllCoroutines();
   }

   public void ChangeWeapon(WeaponType targetWeapon)
   {
       if(curWeapon == weaponDic[targetWeapon] ) return;
       
       curWeapon = weaponDic[targetWeapon];
   }

   public void UpgradeWeapon()
   {
       if(curWeapon.isUpgraded) return;
       curWeapon = weaponDic[curWeapon.upgradedWeaponType];
   }
   
   //TODO:现在将该功能直接强硬地写在了需要出现的地方，不合理
   public void WeaponWork()
   {
       switch (curWeapon.weaponType)
       {
           case WeaponType.BrassKnuckles:
               BrassKnucklesEffect();
               break;
           case WeaponType.BrassKnucklesPlus:
               BrassKnucklesEffect();
               break;
           case WeaponType.Chair:
               ChairEffect();
               break;
           case WeaponType.ChairPlus:
               ChairEffect();
               break;
           case WeaponType.Nunchucks:
               NunchucksEffect();
               break;
           case WeaponType.NunchucksPlus:
               NunchucksEffect();
               break;
       }
   }

   private void BrassKnucklesEffect()
   {
       if (curWeapon.weaponType != WeaponType.BrassKnuckles && curWeapon.weaponType != WeaponType.BrassKnucklesPlus) return;
       
       if(!CardManager.Instance.CheckLastCardType(CardType.AttackCard)) return;
       
       if (CardManager.Instance.CalCardNumInThisRound(CardType.AttackCard) % curWeapon.weaponValue01 == 0)
       {
          GiveBuff(BuffProvider.Instance.EasyHurt,(int)curWeapon.weaponValue03,
              BattleManager.Instance.GetRandomTargerEnemy((int)curWeapon.weaponValue02).ToArray());
       }
   }
   private void ChairEffect()
   {
       if (curWeapon.weaponType != WeaponType.Chair && curWeapon.weaponType != WeaponType.ChairPlus) return;
       
       if(!CardManager.Instance.CheckLastCardType(CardType.AttackCard)) return;
       
       Defend((int)curWeapon.weaponValue01,curWeapon.weaponValue02,false);
   }
   private void NunchucksEffect()
   {
       if (curWeapon.weaponType != WeaponType.Nunchucks && curWeapon.weaponType != WeaponType.BrassKnuckles) return;
       
       if(!CardManager.Instance.CheckLastCardType(CardType.AttackCard) || 
          !CardManager.Instance.CheckCurCardType(CardType.AttackCard)) return;
       
       BattleManager.Instance.GetEnergy((int)curWeapon.weaponValue01);
   }
   public bool MaceEffect()
   {
       if (curWeapon.weaponType != WeaponType.MacePlus && curWeapon.weaponType != WeaponType.Mace) return false;
       
       return CardManager.Instance.CalCardNumInThisRound(CardType.AttackCard) >= curWeapon.weaponValue01;
   }
}


