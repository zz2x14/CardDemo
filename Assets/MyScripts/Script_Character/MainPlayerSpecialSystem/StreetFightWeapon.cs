using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "StreetFighterWeapon/NewStreetFighterWeapon",fileName = "NewStreetFighterWeapon")]
public class StreetFightWeapon : ScriptableObject
{
    public int weaponId;
    [FormerlySerializedAs("WeaponType")] public WeaponType weaponType;
    public WeaponType upgradedWeaponType;
    public bool isUpgraded;
    public float weaponAttackPoint;
    public float weaponValue01;
    public float weaponValue02;
    public float weaponValue03;
}

public enum WeaponType
{
    BrassKnuckles,
    BrassKnucklesPlus,
    Chair,
    ChairPlus,
    Nunchucks,
    NunchucksPlus,
    Mace,
    MacePlus,
    GolfStick,
    GolfStickPlus,
    Catapult,
    CatapultPlus
}
