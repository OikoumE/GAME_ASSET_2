using System.Collections.Generic;
using StateMachine;
using TMPro;
using UnityEngine;

public class SettingsMenuHandler : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private TMP_InputField inputField;

    private void Start()
    {
        inputField.text = "50";
        dropdown.options = new List<TMP_Dropdown.OptionData>();
        foreach (var resolution in Screen.resolutions)
        {
            var abs = new TMP_Dropdown.OptionData
            {
                text = resolution.ToString()
            };
            dropdown.options.Add(abs);
        }
    }


    private void Update()
    {
    }

    public void OnCancelButton()
    {
        Debug.Log("OnCancelButton");
    }

    private float ValidateInputFieldValues()
    {
        float inputtedSens = 50;
        if (inputField.text.Length != 0)
        {
            inputtedSens = float.Parse(inputField.text);
            if (inputtedSens > 100) inputField.text = "100";
            else if (inputtedSens < 0.1) inputField.text = "0.1";
            Debug.Log("Validated sens: " + inputtedSens);
            return Mathf.Clamp(inputtedSens, 100, .1f);
        }

        Debug.Log("Validated sens: " + inputtedSens);
        inputField.text = inputtedSens.ToString();
        return inputtedSens;
    }


    public void OnInputFieldChanged()
    {
        var sens = ValidateInputFieldValues();
        GameStateMachine.Instance.SetAllCameraSens(sens);
    }

    public void OnApplyButton()
    {
        Debug.Log("OnApplyButton");
        // handle mouse sens
        var sens = ValidateInputFieldValues();
        GameStateMachine.Instance.SetAllCameraSens(sens);

        //TODO handle screen res

        //TODO Handle fullscreen toggle
    }
}