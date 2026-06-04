using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stats
{
  public int strength;
  public int magic;
  public int endurance;
  public int agility;
  public int luck;
  public int sp;
  public int hp;
}
public class Demon : MonoBehaviour
{
  public string demonName;
  public Sprite demonSprite;
  public Stats stats;
  public List<Type> weaknesses;
  public List<Type> resistances;
  public List<Type> immunities;
  public List<Skills> skills;
  private int attackDuration = 0; // negative if debuff, positive if both (max: 3/-3)
  private int defenseDuration = 0; // negative if debuff, positive if both (max: 3/-3)
  private int accuracyDuration = 0; // negative if debuff, positive if both (max: 3/-3)

  /***
  * Processes skill and returns damage dealt or healing done
  ***/
  public int ReceiveSkill(Skills skill)
  {
    // This method can be expanded to handle buffs, debuffs, healing, etc. based on skill type and modifier
    if (skill.type == Type.Heal)
    {
      int healAmt = skill.weight switch
      {
        DamageWeight.Light => GLOBAL.LIGHT_HEAL,
        DamageWeight.Medium => GLOBAL.MEDIUM_HEAL,
        DamageWeight.Heavy => GLOBAL.HEAVY_HEAL,
        _ => 0,
      };

      stats.hp += healAmt;
      return healAmt;
    }
    else if (skill.type == Type.Buff)
    {
      switch (skill.modifier)
      {
        case Modifier.Attack:
          attackDuration = attackDuration < 0 ? 0 : 3;
          break;
        case Modifier.Defense:
          defenseDuration = defenseDuration < 0 ? 0 : 3; // Increase defense for next 3 turns
          break;
        case Modifier.Accuracy:
          accuracyDuration = accuracyDuration < 0 ? 0 : 3; // Increase accuracy for next 3 turns
          break;
      }
      return 0;
    }
    else if (skill.type == Type.Debuff)
    {
      switch (skill.modifier)
      {
        case Modifier.Attack:
          attackDuration = attackDuration > 0 ? 0 : -3;
          break;
        case Modifier.Defense:
          defenseDuration = defenseDuration > 0 ? 0 : -3; // Increase defense for next 3 turns
          break;
        case Modifier.Accuracy:
          accuracyDuration = accuracyDuration > 0 ? 0 : -3; // Increase accuracy for next 3 turns
          break;
      }
      return 0;
    }

    return TakeDamage(skill);
  }
  public int TakeDamage(Skills skill)
  {
    // accuracy check
    int finalAccuracy = skill.accuracy;
    if (accuracyDuration != 0)
    {
      finalAccuracy = Mathf.RoundToInt(finalAccuracy * (accuracyDuration < 0 ? 0.9f : 1.1f)); // Buff increases accuracy by 50%, debuff decreases by 50%
      accuracyDuration += accuracyDuration > 0 ? -1 : 1; // Decrease duration each turn
    } 
    int hitTest = Random.Range(1, 101); // Random number between 1 and 100
    if (hitTest > finalAccuracy)
    {
      Debug.Log($"{demonName} evaded the attack!");
      return 0; 
    }

    int damage = 0;
    // Calc base damage based on skill type and weight
    damage = skill.weight switch
    {
      DamageWeight.Light => GLOBAL.LIGHT_DAMAGE,
      DamageWeight.Medium => GLOBAL.MEDIUM_DAMAGE,
      DamageWeight.Heavy => GLOBAL.HEAVY_DAMAGE,
      _ => 0,
    };

    // Attack formulas
    if (skill.type == Type.Physical)
      damage = stats.strength * damage / 15;
    else
      damage = (int)(0.004f * (5 *(stats.magic + 20) * (24 * damage * (1/255f)) + 1)); 

    // Modify damage
    if (weaknesses.Contains(skill.type))
      damage = Mathf.RoundToInt(damage * 1.5f); // 50% more damage
    else if (resistances.Contains(skill.type))
      damage = Mathf.RoundToInt(damage * 0.5f); // 50% less damage
    else if (immunities.Contains(skill.type))
      damage = 0; 

    // apply defense buff/debuff
    if (defenseDuration != 0)
    {
      damage = Mathf.RoundToInt(damage * (defenseDuration < 0 ? 1.5f : 0.5f)); // Buff increases damage by 50%, debuff decreases by 50%
      defenseDuration += defenseDuration > 0 ? -1 : 1; // Decrease duration each turn
    }

    return damage; // Return for ui/player feedback and so i can ctrl c v this to player demon
  }
}
