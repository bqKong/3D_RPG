using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack",menuName ="Attack/Attack Data")]
public class AttackData_SO : ScriptableObject
{
    [Tooltip("攻击范围")]
    public float attackRange;
    [Tooltip("技能范围")]
    public float skillRange;
    [Tooltip("技能冷却时间")]
    public float coolDown;

    public int minDamge;
    public int maxDamge;

    [Tooltip("暴击加成百分比")]
    public float criticalMultiplier;
    [Tooltip("暴击率")]
    public float criticalChance;

}
