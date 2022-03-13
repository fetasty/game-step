using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public CharacterData_SO characterData;
    public AttackData_SO attackData;

    [HideInInspector]
    public bool isCritical;

    #region Read from Data_SO
    public int MaxHealth
    {
        get => characterData == null ? 0 : characterData.maxHealth;
        set { characterData.maxHealth = value; }
    }
    public int CurrentHealth
    {
        get => characterData == null ? 0 : characterData.currentHealth;
        set { characterData.currentHealth = value; }
    }
    public int BaseDefence
    {
        get => characterData == null ? 0 : characterData.baseDefence;
        set { characterData.baseDefence = value; }
    }
    public int CurrentDefence
    {
        get => characterData == null ? 0 : characterData.currentDefence;
        set { characterData.currentDefence = value; }
    }
    #endregion

    #region Character Combat
    public void TakeDamage(CharacterStats attacker, CharacterStats defener)
    {
        int damage = Mathf.Max(attacker.CurrentDamage() - defener.CurrentDefence, 0);
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);

        if (attacker.isCritical)
        {
            Debug.Log($"{attacker.gameObject.name} 暴击, 实际伤害: {damage}");
            defener.GetComponent<Animator>().SetTrigger("Hit");
        }
        // TODO:更新UI
        // TODO:经验更新
    }

    private int CurrentDamage()
    {
        float coreDamage = UnityEngine.Random.Range(attackData.minDamage, attackData.maxDamage);

        if (isCritical)
        {
            coreDamage *= attackData.criticalMultiplier;
        }

        return (int)coreDamage;
    }
    #endregion
}
