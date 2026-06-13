using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class SwitchDemonsVisual : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject Player;
    private Player player;
    public InputActionReference SwitchAction;
    public InputActionReference SelectAction;
    public InputActionReference EscapeAction;
    private DemonParty demonParty;
    [SerializeField] private GameObject CombatUI;
    [SerializeField] private Image DemonImage;
    [SerializeField] private TextMeshProUGUI DemonName;
    [SerializeField] private TextMeshProUGUI[] Affinities;
    [SerializeField] private TextMeshProUGUI[] Skills;

    [Header("Affinity Marks")]
    [SerializeField] private string defaultMark = "";
    [SerializeField] private string immuneMark = "X";
    [SerializeField] private string weakMark = "+";
    [SerializeField] private string resistMark = "-";

    private Demon[] party;

    private int currIndex = 0;
    private int partyCount;

  void Start()
  {
    if (!Player || Player == null)
    {
        Debug.LogWarning("SwitchDemonsUI: Player reference not set");
        return;
    }
    demonParty = Player.GetComponent<DemonParty>();
    player = Player.GetComponent<Player>();
    if (demonParty == null)
        Debug.LogError("Failed to get Demon Party");

    partyCount = demonParty.GetCurrentParty().Length;
    party = demonParty.GetCurrentParty();

    UpdateDisplay();
  }

  void OnEnable()
  {
    SwitchAction.action.Enable();
    SelectAction.action.Enable();
    EscapeAction.action.Enable();
    UpdateDisplay();
  }

  void OnDisable()
  {
    SwitchAction.action.Disable();
    SelectAction.action.Disable();
    EscapeAction.action.Disable();
  }

  void Update()
  {
    if (SelectAction.action.WasPressedThisFrame())
    {  
        Debug.Log("Switching to new demon");
        SelectDemon();
    } else if (SwitchAction.action.WasPressedThisFrame())
    {
        int advanceValue = (int)SwitchAction.action.ReadValue<float>();
        if (advanceValue > 0)
        {
            Debug.Log("Update towards next demon");
            UpdateDemon(true);
        }
        else if (advanceValue < 0)
        {
            Debug.Log("Update towards previous demon");
            UpdateDemon(false);
        }
    } else if (EscapeAction.action.WasPressedThisFrame())
    {
        Debug.Log("Leaving page");
        CombatUI.SetActive(true);
        gameObject.SetActive(false);
    }
  }

  void UpdateDisplay(Demon demon = null)
{
    // get current demon if null
    if (demon == null)
        demon = Player.GetComponent<DemonParty>().GetCurrentDemon();

    DemonImage.sprite = demon.demonSprite;
    DemonName.text = demon.name;

    // update skills
    UpdateSkills(demon);

    // update affinities
    UpdateAffinities(demon);
    

}

/* HELPER FUNCTIONS */
// true for next demon, false for prev demon
void UpdateDemon(bool nextDemon = true)
{
    // update index
    if (nextDemon)
        currIndex = (currIndex + 1 + partyCount) % partyCount;
    else 
        currIndex = (currIndex - 1 + partyCount) % partyCount;

    UpdateDisplay(party[currIndex]);
}
void SelectDemon()
{
    player.SwitchDemon(party[currIndex]);
}
void UpdateSkills(Demon currDemon)
{
    Debug.Log("Skills array length: " + Skills.Length);
for (int i = 0; i < Skills.Length; i++)
{
    Debug.Log($"Skills[{i}] = {(Skills[i] == null ? "NULL" : Skills[i].name)}");
}

    string NO_SKILL_TEXT = "-";
    if (currDemon.skills == null || currDemon.skills.Count == 0)
        Debug.LogWarning("demon skills are null or 0");
    int totalSkills = currDemon.skills.Count;
    for (int i = 0; i < Skills.Length; i++)
    {
        Debug.Log("On iteration: " + i);
        string currSkillText = NO_SKILL_TEXT;
        
        if (i < totalSkills)
        {
            currSkillText = currDemon.skills[i].skillName;
            Debug.Log("Changing text to: " + currSkillText);

            
        }
        TextMeshProUGUI curr = Skills[i];
        if (curr == null)
            {
                Debug.LogError("curr skills is empty");
            }
        curr.text = currSkillText;
    }
}
void UpdateAffinities(Demon currDemon)
{
    for (int i = 0; i < Affinities.Length; i++)
    {
        // determine marking
        string marking = defaultMark;
        switch((Type)i)
        {
            case Type.Physical:
                if (currDemon.immunities.Contains(Type.Physical))
                    marking = immuneMark;
                else if (currDemon.resistances.Contains(Type.Physical))
                    marking = resistMark;
                else if (currDemon.weaknesses.Contains(Type.Physical))
                    marking = weakMark;
                break;
            case Type.Fire:
                if (currDemon.immunities.Contains(Type.Fire))
                    marking = immuneMark;
                else if (currDemon.resistances.Contains(Type.Fire))
                    marking = resistMark;
                else if (currDemon.weaknesses.Contains(Type.Fire))
                    marking = weakMark;
                break;
            case Type.Ice:
                if (currDemon.immunities.Contains(Type.Ice))
                    marking = immuneMark;
                else if (currDemon.resistances.Contains(Type.Ice))
                    marking = resistMark;
                else if (currDemon.weaknesses.Contains(Type.Ice))
                    marking = weakMark;
                break;
            case Type.Wind:
                if (currDemon.immunities.Contains(Type.Wind))
                    marking = immuneMark;
                else if (currDemon.resistances.Contains(Type.Wind))
                    marking = resistMark;
                else if (currDemon.weaknesses.Contains(Type.Wind))
                    marking = weakMark;
                break;
            case Type.Electric:
                if (currDemon.immunities.Contains(Type.Electric))
                    marking = immuneMark;
                else if (currDemon.resistances.Contains(Type.Electric))
                    marking = resistMark;
                else if (currDemon.weaknesses.Contains(Type.Electric))
                    marking = weakMark;
                break;
            case Type.Light:
                if (currDemon.immunities.Contains(Type.Light))
                    marking = immuneMark;
                else if (currDemon.resistances.Contains(Type.Light))
                    marking = resistMark;
                else if (currDemon.weaknesses.Contains(Type.Light))
                    marking = weakMark;
                break;
            case Type.Dark:
                if (currDemon.immunities.Contains(Type.Dark))
                    marking = immuneMark;
                else if (currDemon.resistances.Contains(Type.Dark))
                    marking = resistMark;
                else if (currDemon.weaknesses.Contains(Type.Dark))
                    marking = weakMark;
                break;
        }

        Affinities[i].text = marking;
    }
}



}
