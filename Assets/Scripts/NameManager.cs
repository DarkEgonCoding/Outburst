using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NameManager : MonoBehaviour
{
    public const int MAX_NAME_LENGTH = 11;

    [SerializeField] private TMP_InputField nameInputField;

    private void Awake()
    {
        if (nameInputField != null)
            nameInputField.characterLimit = MAX_NAME_LENGTH;
    }

    private void OnEnable()
    {
        // Automatically focus input field when Name Picker shows
        if (nameInputField != null)
        {
            nameInputField.text = "";
            nameInputField.Select();
            nameInputField.ActivateInputField();
        }
    }

    private void Start()
    {
        if (nameInputField != null)
        {
            // Clear field at start
            nameInputField.text = "";

            // Listen for Enter/Return key
            nameInputField.onSubmit.AddListener(delegate { OnSubmitName(); });
        }
    }

    public void HandleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            OnSubmitName();
        }
    }

    public void NameChosen(string playerName)
    {
        Debug.Log($"Player name chosen: {playerName}");
        GameController.instance.SetupGame(playerName);
    }

    private void OnSubmitName()
    {
        string playerName = nameInputField.text.Trim();

        if (!string.IsNullOrEmpty(playerName))
        {
            NameChosen(playerName);
        }
        else
        {
            Debug.LogWarning("Player name cannot be empty!");
        }
    }

    public void ForceUppercase()
    {
        if (nameInputField.text != nameInputField.text.ToUpper())
        {
            int caretPosition = nameInputField.caretPosition; // save cursor
            nameInputField.text = nameInputField.text.ToUpper();
            nameInputField.caretPosition = caretPosition; // restore cursor
        }
    }
}
