using UnityEngine;

public class Player : Demon
{
    private Stats playerStats;
    [SerializeField] private DemonParty demonParty; // Reference to the player's demon party
    [SerializeField] private int MAX_HEALTH = 100; // Maximum health for the player
    [SerializeField] private int MAX_SP = 50; // Maximum mana for the player
    private int currHp;
    private int currSp;

    void Start()
    {
        if (demonParty == null)
        {
            Debug.LogError("DemonParty reference not set on Player script.");
            demonParty = gameObject.GetComponent<DemonParty>();
            // return;
        }
        if (demonParty.GetCurrentParty() == null)
        {
            Debug.LogError("current party is null");
        }
        // for (int i = 0; i < demonParty.GetCurrentParty().Length; i++)
        // {
        //     SwitchDemon(demonParty.GetCurrentParty()[i]); // test
        // }
        SwitchDemon(demonParty.GetCurrentParty()[0]);

        // testing modifier
        Demon testAttacker = new Demon(demonParty.GetCurrentDemon());
        testAttacker.stats.strength = 30;
        if (testAttacker == null)
            Debug.LogError("testAttacker is null");
        Skills testModifierAttack = new Skills
        {
            skillName = "Attack Up",
            description = "Attack increased for 3 turns",
            type = Type.Buff,
            weight = DamageWeight.None,
            modifier = Modifier.Attack,
            accuracy = 100
        };
        // test attacker has attack increased for 3 turns
        testAttacker.ReceiveSkill(testModifierAttack, null);
        if (testAttacker.attackDuration == 0)
            Debug.LogError("Test Attacker's attack hasn't been increased. It's at " + testAttacker.attackDuration);

        // testing damage
        Skills REGULAR_ATTACK = new Skills
        {
        skillName = "Attack",
        description = "Light Physical damage to 1 foe",
        type = Type.Physical,
        weight = DamageWeight.Light,
        modifier = Modifier.None,
        accuracy = 95
        };

        // first hit, increased
        int damage = demonParty.GetCurrentDemon().ReceiveSkill(REGULAR_ATTACK, testAttacker);
        Debug.Log($"1) The demon {demonParty.GetCurrentDemon().demonName} wouldve taken {damage} damage");

        // second hit, increased
        damage = demonParty.GetCurrentDemon().ReceiveSkill(REGULAR_ATTACK, testAttacker);
        Debug.Log($"2) The demon {demonParty.GetCurrentDemon().demonName} wouldve taken {damage} damage");

        // third hit, increased
        damage = demonParty.GetCurrentDemon().ReceiveSkill(REGULAR_ATTACK, testAttacker);
        Debug.Log($"3) The demon {demonParty.GetCurrentDemon().demonName} wouldve taken {damage} damage");

        // fourth hit, normal
        if (testAttacker.attackDuration != 0)
            Debug.LogError("Test Attacker's attack hasn't been reset to 0. It's at " + testAttacker.attackDuration);
        damage = demonParty.GetCurrentDemon().ReceiveSkill(REGULAR_ATTACK, testAttacker);
        Debug.Log($"4) The demon {demonParty.GetCurrentDemon().demonName} wouldve taken {damage} damage");
    }

    /* SWAP DEMON METHOD */
    public void SwitchDemon(Demon newDemon)
    {
        demonParty.SetCurrentDemon(newDemon); // Update the current active demon
        UpdatePlayerStats(); // Update player stats based on the new active demon
    }
    void UpdatePlayerStats()
    {
        // Update player stats based on the currently active demon in the party
        Debug.Log($"Updating player stats for current demon: {demonParty.GetCurrentDemon().demonName}");
        Demon currentDemon = demonParty.GetCurrentDemon();
        if (currentDemon != null)
        {
            playerStats = currentDemon.stats; // Assuming Demon has a Stats property
            playerStats.hp = currHp; // Set health to current at the start of combat
            playerStats.sp = currSp; // Set mana to current at the start of combat
        }
    }



}
