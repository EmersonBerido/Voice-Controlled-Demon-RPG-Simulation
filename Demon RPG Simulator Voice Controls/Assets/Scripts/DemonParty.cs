using UnityEngine;
using System.Collections.Generic;
public enum Type
{
    Physical,
    Fire,
    Ice,
    Electric,
    Wind,
    Light,
    Dark,
    Almighty,
    Buff,
    Debuff,
    Heal
}
public enum DamageWeight
{
    None,
    Light,
    Medium,
    Heavy
}
public enum Modifier
{
    None,
    Attack,
    Defense,
    Accuracy
}
[System.Serializable]
public class Skills
{
    public string skillName;
    public string description;
    public Type type;
    public DamageWeight weight;
    public Modifier modifier;
    public int accuracy;

}

public class DemonParty : MonoBehaviour
{
    [Header("Party Demons")]
    [SerializeField] private GameObject[] Demons; // List to hold the player's current party of demons
    private Demon[] currentParty; // Array to hold the current party of demons during combat
    private Demon currentDemon; // Reference to the currently active demon in combat
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // make a COPY of the party demons to avoid modifying the original array
        currentParty = new Demon[Demons.Length];
        for (int i = 0; i < Demons.Length; i++)
            currentParty[i] = Demons[i].GetComponent<Demon>();
        currentDemon = currentParty[0]; // Set the first demon as the active demon at the start of combat
    }

    Demon GetCurrentDemon() => currentDemon; // Method to get the currently active demo

}
