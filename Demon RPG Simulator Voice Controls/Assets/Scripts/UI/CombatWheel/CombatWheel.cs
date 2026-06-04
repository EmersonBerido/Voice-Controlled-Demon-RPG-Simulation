using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class ActionWheelButton
{
    public GameObject buttonObject; // Reference to the button GameObject
    public string actionName; // Name of the action (e.g., "Attack", "Skill", etc.)
    public string description; // Description of the action for tooltips or UI display
    public ActionWheelButton(GameObject buttonObject, string actionName, string description)
    {
        this.buttonObject = buttonObject;
        this.actionName = actionName;
        this.description = description;
    }
}
public class CombatWheel : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private GameObject AttackButton;
    [SerializeField] private GameObject SkillButton;
    [SerializeField] private GameObject BagButton;
    [SerializeField] private GameObject PartyButton;
    [SerializeField] private GameObject PersonaButton;
    [SerializeField] private GameObject GuardButton;
    [SerializeField] private GameObject RunButton;

    [Header("Display References")]
    [SerializeField] private GameObject PlayerActionsUI; // Parent display
    [SerializeField] private GameObject ActionDescriptionText; // Text element to show action descriptions
    [SerializeField] private GameObject ActionNameText; // Text element to show action names


    // Button format
    private List<ActionWheelButton> actionWheelButtons; // List to hold button data and references

    // Original Positions of buttons 
    private List<Vector2> buttonPositions;
    private int selectedButtonIndex = 0; // Index of the currently selected button

    public InputActionReference AdvanceAction; // axis input

    void OnEnable() => AdvanceAction.action.Enable();
    void OnDisable() => AdvanceAction.action.Disable();


    void Start()
    {
        // Initialize buttons list
        actionWheelButtons = new List<ActionWheelButton>
        {
            new ActionWheelButton(AttackButton, "Attack", "Perform a physical attack on the enemy."),
            new ActionWheelButton(SkillButton, "Skill", "Use assigned Demon's special ability."),
            new ActionWheelButton(BagButton, "Item", "Use an item."),
            new ActionWheelButton(PartyButton, "Party", "Manage your party members and their actions."),
            new ActionWheelButton(PersonaButton, "Demon", "Switch to a different Demon."),
            new ActionWheelButton(GuardButton, "Guard", "Defend to reduce damage."),
            new ActionWheelButton(RunButton, "Run", "End the battle.")
        };

        // Store original positions of buttons
        buttonPositions = new List<Vector2>();
        foreach (ActionWheelButton actionButton in actionWheelButtons)
        {
            buttonPositions.Add(actionButton.buttonObject.GetComponent<RectTransform>().anchoredPosition);
        }

        // update initial button
        Button selectedButton = actionWheelButtons[selectedButtonIndex].buttonObject.GetComponent<Button>(); // Get the currently selected
        selectedButton.Select(); // Select the new main button
        ActionNameText.GetComponent<TMP_Text>().text = actionWheelButtons[selectedButtonIndex].actionName;
        ActionDescriptionText.GetComponent<TMP_Text>().text = actionWheelButtons[selectedButtonIndex].description;
      
    }

    void Update()
    {
        // Check if the advance action is triggered
        if (AdvanceAction.action.WasPressedThisFrame())
        {
            // Debug.Log("Advance action triggered");
            // //Log current value
            // int currentValue = (int)AdvanceAction.action.ReadValue<float>();
            // Debug.Log("Current Value: " + currentValue);
            UpdateButtonLayout();
        }
    }

    void UpdateButtonLayout()
    {
        int advanceValue = (int)AdvanceAction.action.ReadValue<float>();
        if (advanceValue > 0)
            ForwardAdvance();
        else if (advanceValue < 0)
            BackwardAdvance();

        Button selectedButton = actionWheelButtons[selectedButtonIndex].buttonObject.GetComponent<Button>(); // Get the currently selected
        selectedButton.Select(); // Select the new main button
        Debug.Log("Main button is now: " + actionWheelButtons[selectedButtonIndex].buttonObject.name); // Log the new main button
        ActionNameText.GetComponent<TMP_Text>().text = actionWheelButtons[selectedButtonIndex].actionName;
        ActionDescriptionText.GetComponent<TMP_Text>().text = actionWheelButtons[selectedButtonIndex].description;

    }

    void ForwardAdvance()
    {
        Vector2 nextMovement = buttonPositions[actionWheelButtons.Count - 1]; // Store the first button's position to loop back later
        for (int i = actionWheelButtons.Count - 1; i >= 0; i--)
        {
            // Calculate new position based on the original position and the advance action value
            int newPositionIndex = i - 1 < 0 ? actionWheelButtons.Count - 1 : i - 1; // Loop back to the last button

            // temp values
            Vector2 tempPosition = buttonPositions[newPositionIndex]; // Store the new position for the next iteration

            // visually update the button positions
            actionWheelButtons[newPositionIndex].buttonObject.GetComponent<RectTransform>().anchoredPosition = nextMovement; // Move the next button to the initial position
            buttonPositions[newPositionIndex] = nextMovement; // Update the button positions list

            // Update the initial position and button for the next iteration
            nextMovement = tempPosition; // Update initial position for the next iteration
        }
        selectedButtonIndex = (selectedButtonIndex - 1 + actionWheelButtons.Count) % actionWheelButtons.Count; // Update the selected button index
    }

    void BackwardAdvance()
    {
        Vector2 initialPosition = buttonPositions[0]; // Store the first button's position to loop back later
        for (int i = 0; i < actionWheelButtons.Count; i++)
        {
            // Calculate new position based on the original position and the advance action value
            int newPositionIndex = i + 1 >= actionWheelButtons.Count ? 0 : i + 1; // Loop back to the first button

            // temp values
            Vector2 tempPosition = buttonPositions[newPositionIndex]; // Store the new position for the next iteration

            // visually update the button positions
            actionWheelButtons[newPositionIndex].buttonObject.GetComponent<RectTransform>().anchoredPosition = initialPosition; // Move the next button to the initial position
            buttonPositions[newPositionIndex] = initialPosition; // Update the button positions list
            // actionWheelButtons[newPositionIndex] = actionWheelButtons[i]; // Update the buttons list

            // Update the initial position and button for the next iteration
            initialPosition = tempPosition; // Update initial position for the next iteration
        }
        selectedButtonIndex = (selectedButtonIndex + 1) % actionWheelButtons.Count; // Update the selected button index
    }

}
