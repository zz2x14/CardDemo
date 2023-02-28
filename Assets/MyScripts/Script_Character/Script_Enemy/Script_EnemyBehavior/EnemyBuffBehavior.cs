using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyBuffBehavior : EnemyBehavior
{
    [SerializeField] protected List<CharacterBuff> buffs = new List<CharacterBuff>();
}
