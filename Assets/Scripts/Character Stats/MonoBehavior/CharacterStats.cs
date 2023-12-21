using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public CharacterData_SO templateData;

    public CharacterData_SO characterData;

    public AttackData_SO attackData;

    [HideInInspector]
    public bool isCritical;

    private void Awake()
    {
        //一份copy 的data，从模板中复制出来一份，防止大家共用一份导致一起死亡
        if(templateData != null) 
        {
            characterData = Instantiate(templateData);
        }

    }

    //属性的标准写法，避免CharacterStats.characterData.XXX;
    #region Read from Data_SO
    public int MaxHealth
    {
        get
        {
            if (characterData != null)
                return characterData.maxHealth;
            else
                return 0;
        }
        set
        { characterData.maxHealth = value; }

    }

    public int CurrentHealth
    {
        get
        {
            if (characterData != null)
                return characterData.currentHealth;
            else
                return 0;
        }
        set
        { characterData.currentHealth = value; }

    }

    public int BaseDefence
    {
        get
        {
            if (characterData != null)
                return characterData.baseDefence;
            else
                return 0;
        }
        set
        { characterData.baseDefence = value; }

    }

    public int CurrentDefence
    {
        get
        {
            if (characterData != null)
                return characterData.currentDefence;
            else
                return 0;
        }
        set
        { characterData.currentDefence = value; }

    }


    #endregion


    #region Character Combat

    public void TakeDamage(CharacterStats attacker, CharacterStats defener)
    {
        int damage = Mathf.Max(attacker.CurrentDamage() - defener.CurrentDefence,0);
        CurrentHealth = Mathf.Max(CurrentHealth - damage,0);

        //如果暴击，播放受击动画
        if (isCritical)
        {
            defener.GetComponent<Animator>().SetTrigger("Hit");
        }

        //TODO:UPDATE UUI
        //TODO:经验UPDATE
    }

    /// <summary>
    /// 真实攻击力
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    private int CurrentDamage()
    {
        float coreDamge = UnityEngine.Random.Range(attackData.minDamge, attackData.maxDamge);

        if (isCritical)
        {
            //暴击伤害
            coreDamge *= attackData.criticalMultiplier;
        }

        return (int) coreDamge;
    }


    #endregion


}
