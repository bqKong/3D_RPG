using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Data", menuName = "Character Stats/Data")]
public class CharacterData_SO : ScriptableObject
{
    [Header("Stats Info")]
    //最大血量
    public int maxHealth;
    //当前血量
    public int currentHealth;
    //基础防御
    public int baseDefence;
    //当前防御
    public int currentDefence;

    [Header("Kill")]
    public int killPoint;

    [Header("Level")]
    public int currentLevel;

    public int maxLevel;

    public int baseExp;

    public int currentExp;

    public float levelBuff;

    //等级系数因子
    public float LevelMultiplier
    {
        get { return (1 + currentLevel - 1) * levelBuff; }
    }

    public void UpdateExp(int point)
    {
        currentExp += point;

        if (currentExp >= baseExp)
        {
            LevelUp();
        }

    }

    private void LevelUp()
    {
        //所有你想提升的数据方法,
        currentLevel = Mathf.Clamp(currentLevel + 1, 1, maxLevel);
        baseExp += (int)(baseExp * LevelMultiplier);

        //每次提升百分之10
        maxHealth = (int)(maxHealth * 0.1f);
        currentHealth = maxHealth;
        Debug.Log($"Level Up!  Current Level: {currentLevel},Max Health{maxHealth} ");
    }

}
