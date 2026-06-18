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

  public Stats(Stats other)
  {
    strength = other.strength;
    magic = other.magic;
    endurance = other.endurance;
    agility = other.agility;
    luck = other.luck;
    sp = other.sp;
    hp = other.hp;
  }
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
  public int attackDuration = 0; // negative if debuff, positive if both (max: 3/-3)
  private int defenseDuration = 0; // negative if debuff, positive if both (max: 3/-3)
  private int accuracyDuration = 0; // negative if debuff, positive if both (max: 3/-3)
  private bool isGuarding = false;

  public Demon() { }

  // Copy constructor
  public Demon(Demon other)
  {
    demonName = other.demonName;
    demonSprite = other.demonSprite;
    stats = new Stats(other.stats); 
    weaknesses = new List<Type>(other.weaknesses);
    resistances = new List<Type>(other.resistances);
    immunities = new List<Type>(other.immunities);
    skills = new List<Skills>(other.skills);
  }

  /*
    Processes skill and returns damage dealt or healing done
    Calculates damage that a demon/player would take
    BattleManager handles the demon/player receiving the damage
  */
  public int ReceiveSkill(Skills skill, Demon attacker)
  {
    int damageTaken = 0;
    if (attacker == null)
      Debug.LogError("Attacker is NULL");

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

      isGuarding = false; // Remove guard if healed

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
      isGuarding = false; // Remove guard if buffed
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
      isGuarding = false; // Remove guard if debuffed
      return 0;
    } else
    {
      damageTaken = TakeDamage(skill, attacker);
    }

    if (attacker != null)
      attacker.DecreaseModifierDuration();

    return damageTaken;
  }

  /**
    Calculates damage based off accuracy, defense, and attack

    TODO: add demon's endurance into the mix
  */
  public int TakeDamage(Skills skill, Demon attacker)
  {
    // accuracy check
    int finalAccuracy = skill.accuracy;
    if (accuracyDuration != 0)
      finalAccuracy = Mathf.RoundToInt(finalAccuracy * (accuracyDuration < 0 ? 0.9f : 1.1f)); // Buff increases accuracy by 50%, debuff decreases by 50%

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
    // Debug.Log("Damage is currently at "+ damage);

    // Attack formulas
    if (skill.type == Type.Physical)
      damage = attacker.stats.strength * damage / 15;
    else
      damage = (int)(0.004f * (5 *(attacker.stats.magic + 20) * (24 * damage * (1/255f)) + 1)); 
    // Debug.Log("Damage is currently at "+ damage);

    // Modify damage
    if (weaknesses.Contains(skill.type))
      damage = Mathf.RoundToInt(damage * 1.5f); // 50% more damage
    else if (resistances.Contains(skill.type))
      damage = Mathf.RoundToInt(damage * 0.5f); // 50% less damage
    else if (immunities.Contains(skill.type))
      damage = 0; 
    // Debug.Log("Damage is currently at "+ damage);

    // apply attack buff
    if (attacker.attackDuration != 0)
      damage = Mathf.RoundToInt(damage * (attackDuration > 0 ? 1.5f : 0.5f));

    // apply defense buff/debuff
    if (defenseDuration != 0)
      damage = Mathf.RoundToInt(damage * (defenseDuration < 0 ? 1.5f : 0.5f)); // Buff increases damage by 50%, debuff decreases by 50%
    // Debug.Log("Damage is currently at "+ damage);

    // apply reduction if guarded
    if (isGuarding)
    {
      damage = Mathf.RoundToInt(damage * 0.5f); // Guarding reduces damage by 50%
      isGuarding = false; // Reset guarding state after one attack
    }
    // Debug.Log("Damage is currently at "+ damage);

    // reduce damage based off demon endurance
    damage -= stats.endurance;

    // alert if death (battle mangager's RemoveEnemyDemon)

    Debug.Log($"{demonName} took {damage} damage from {skill.skillName}! Actual health isnt affected yet");
    return damage > 0 ? damage : 0; // Return for ui/player feedback and so i can ctrl c v this to player demon
  }

  public void DecreaseModifierDuration()
  {
    Debug.Log("Decreasing attacker's modifiers");
    // should be called after the attackers turn
    if (attackDuration != 0)
      attackDuration += attackDuration < 0 ? 1 : -1;
    if (defenseDuration != 0)
      defenseDuration += defenseDuration < 0 ? 1 : -1;
    if (accuracyDuration != 0)
      accuracyDuration += accuracyDuration < 0 ? 1 : -1;
  }
}
