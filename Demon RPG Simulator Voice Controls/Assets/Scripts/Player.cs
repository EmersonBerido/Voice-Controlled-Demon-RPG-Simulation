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
