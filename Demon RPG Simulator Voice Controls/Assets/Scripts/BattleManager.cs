using UnityEngine;
using System.Collections.Generic;

/**
    Handles how enemies take damage

*/

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; } // Singleton instance

    // Wave variables
    private int currentWave = 0;
    [SerializeField] private int MAX_ENEMIES_PER_WAVE = 3;

    // References to demons
    [Header("References")]
    [SerializeField] private List<Demon> EnemyDemons; // List of enemy demon prefabs to spawn from
    [SerializeField] private GameObject player;
    private List<GameObject> turnOrder; // List to hold the turn order of demons in combat
    [SerializeField] private GameObject EnemyListParent;
    private List<GameObject> allDemons;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Instance == null)
        {
            Instance = this; // Set the singleton instance
            DontDestroyOnLoad(gameObject); // Optional: Persist across scenes
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
            return;
        }

        allDemons = new List<GameObject>();
        foreach (Transform demon in EnemyListParent.transform)
        {
            allDemons.Add(demon.gameObject);
        }
        // Debug.Log("Total demons in Enemy List: " + allDemons.Count);

    }

    void SetTurnOrder()
    {
        // decide order based on agility stats of demons in combat (player and enemies)

        // simple sort algorithm
        List<GameObject> tempTurnOrder = new List<GameObject>(turnOrder); // Create a copy of the turn order list
        for (int i = 0; i < tempTurnOrder.Count - 1; i++)
        {
            for (int j = 0; j < tempTurnOrder.Count - i - 1; j++)
            {
                Demon demonA = tempTurnOrder[j].GetComponent<Demon>();
                Demon demonB = tempTurnOrder[j + 1].GetComponent<Demon>();

                if (demonA.stats.agility < demonB.stats.agility)
                {
                    // Swap
                    GameObject temp = tempTurnOrder[j];
                    tempTurnOrder[j] = tempTurnOrder[j + 1];
                    tempTurnOrder[j + 1] = temp;
                }
            }
        }
    }

    void StartWave()
    {
        if (EnemyDemons.Count > 0)
        {
            Debug.Log("Demons already exist, not starting new wave");
            return;
        }

        currentWave++;
        int numEnemies = Mathf.Min(currentWave, MAX_ENEMIES_PER_WAVE); // Increase number of enemies each wave up to max
        for (int i = 0; i < numEnemies; i++)
        {
            // Spawn a random enemy demon from the list and add to combat
            int randomIndex = Random.Range(0, allDemons.Count);
            Demon newDemon = new Demon(allDemons[randomIndex].GetComponent<Demon>()); // Create a new instance of the demon
            EnemyDemons.Add(newDemon);
        }
        
    }

    public void RemoveEnemyDemon(Demon demon)
    {
        EnemyDemons.Remove(demon);
        if (EnemyDemons.Count == 0)
        {
            Debug.Log("Wave cleared! Starting next wave...");
            StartWave();
        }
    }
}
