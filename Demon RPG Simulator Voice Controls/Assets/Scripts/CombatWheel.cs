using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;
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

    // Button format
    private List<GameObject> buttons;  

    // Original Positions of buttons 
    private List<Vector2> buttonPositions;
    private int selectedButtonIndex = 0; // Index of the currently selected button

    public InputActionReference AdvanceAction; // axis input

    void OnEnable() => AdvanceAction.action.Enable();
    void OnDisable() => AdvanceAction.action.Disable();


    void Start()
    {
        // Initialize buttons list
        buttons = new List<GameObject> { AttackButton, SkillButton, BagButton, PartyButton, PersonaButton, GuardButton, RunButton };

        // Store original positions of buttons
        buttonPositions = new List<Vector2>();
        foreach (GameObject button in buttons)
        {
            buttonPositions.Add(button.GetComponent<RectTransform>().anchoredPosition);
        }
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

        Button selectedButton = buttons[selectedButtonIndex].GetComponent<Button>(); // Get the currently selected
        selectedButton.Select(); // Select the new main button
        Debug.Log("Main button is now: " + buttons[selectedButtonIndex].name); // Log the new main button
    }

    void ForwardAdvance()
    {
        // Vector2 initialPosition = buttonPositions[buttons.Count - 1]; // Store the first button's position to loop back later
        // for (int i = 0; i < buttons.Count; i++)
        // {
        //     // Calculate new position based on the original position and the advance action value
        //     int newPositionIndex = i - 1 < 0 ? buttons.Count - 1 : i - 1; // Loop back to the last button

        //     // temp values
        //     Vector2 tempPosition = buttonPositions[newPositionIndex]; // Store the new position for the next iteration

        //     // visually update the button positions
        //     buttons[newPositionIndex].GetComponent<RectTransform>().anchoredPosition = initialPosition; // Move the next button to the initial position
        //     buttonPositions[newPositionIndex] = initialPosition; // Update the button positions list
        //     // buttons[newPositionIndex] = buttons[i]; // Update the buttons list

        //     // Update the initial position and button for the next iteration
        //     initialPosition = tempPosition; // Update initial position for the next iteration
        // }
        // selectedButtonIndex = (selectedButtonIndex - 1 + buttons.Count) % buttons.Count; // Update the selected button index
        // Button selectedButton = buttons[selectedButtonIndex].GetComponent<Button>(); // Get the currently selected button
        // selectedButton.Select(); // Select the new main button
        // Debug.Log("Main button is now: " + buttons[selectedButtonIndex].name); // Log the new main button
        Vector2 nextMovement = buttonPositions[buttons.Count - 1]; // Store the first button's position to loop back later
        for (int i = buttons.Count - 1; i >= 0; i--)
        {
            // Calculate new position based on the original position and the advance action value
            int newPositionIndex = i - 1 < 0 ? buttons.Count - 1 : i - 1; // Loop back to the last button

            // temp values
            Vector2 tempPosition = buttonPositions[newPositionIndex]; // Store the new position for the next iteration

            // visually update the button positions
            buttons[newPositionIndex].GetComponent<RectTransform>().anchoredPosition = nextMovement; // Move the next button to the initial position
            buttonPositions[newPositionIndex] = nextMovement; // Update the button positions list

            // Update the initial position and button for the next iteration
            nextMovement = tempPosition; // Update initial position for the next iteration
        }
        selectedButtonIndex = (selectedButtonIndex - 1 + buttons.Count) % buttons.Count; // Update the selected button index
    }

    void BackwardAdvance()
    {
        Vector2 initialPosition = buttonPositions[0]; // Store the first button's position to loop back later
        for (int i = 0; i < buttons.Count; i++)
        {
            // Calculate new position based on the original position and the advance action value
            int newPositionIndex = i + 1 >= buttons.Count ? 0 : i + 1; // Loop back to the first button

            // temp values
            Vector2 tempPosition = buttonPositions[newPositionIndex]; // Store the new position for the next iteration

            // visually update the button positions
            buttons[newPositionIndex].GetComponent<RectTransform>().anchoredPosition = initialPosition; // Move the next button to the initial position
            buttonPositions[newPositionIndex] = initialPosition; // Update the button positions list
            // buttons[newPositionIndex] = buttons[i]; // Update the buttons list

            // Update the initial position and button for the next iteration
            initialPosition = tempPosition; // Update initial position for the next iteration
        }
        selectedButtonIndex = (selectedButtonIndex + 1) % buttons.Count; // Update the selected button index
    }
}
