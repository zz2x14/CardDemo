using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CharacterAttribute/NewMainPlayerAttribute",fileName = "NewMainPlayerAttribute")]
public class MainPlayerInfo : CharacterInfo
{
    [Header("玩家职业角色私有属性")]
    public int mainPlayerId;
    public int defaultEnergy;
    public MainPlayerEnum mainPlayerEnum;
    public MainPlayerDefaultCardsLibrary playerDefaultCardsLibrary;
}




