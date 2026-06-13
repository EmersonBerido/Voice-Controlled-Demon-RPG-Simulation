using UnityEngine;
using System.Collections.Generic;
public enum Type
{
    Physical,
    Fire,
    Ice,
    Wind,
    Electric,
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
    void Awake()
    {
        // make a COPY of the party demons to avoid modifying the original array
        currentParty = new Demon[Demons.Length];
        for (int i = 0; i < Demons.Length; i++)
            currentParty[i] = Demons[i].GetComponent<Demon>();
        currentDemon = currentParty[0]; // Set the first demon as the active demon at the start of combat

        foreach (Demon demon in currentParty)
        {
            Debug.Log("Demon in party: " + demon.demonName);
        }
    }

    public Demon GetCurrentDemon() => currentDemon; // Method to get the currently active demo
    public void SetCurrentDemon(Demon demon) => currentDemon = demon; // Method to set the currently active demon

    public Demon[] GetCurrentParty() => currentParty; // Method to get the current party of demons

    public void SwitchDemon(string demonName)
    {
        // Find the demon in the current party by name and switch to it
        foreach (Demon demon in currentParty)
        {
            if (demon.demonName == demonName)
            {
                currentDemon = demon;
                Debug.Log("Switched to demon: " + currentDemon.demonName);
                return;
            }
        }
        Debug.LogWarning("Demon with name " + demonName + " not found in party.");
    }


}
