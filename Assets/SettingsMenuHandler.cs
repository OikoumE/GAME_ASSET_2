using System.Collections.Generic;
using StateMachine;
using TMPro;
using UnityEngine;

public class SettingsMenuHandler : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown resolutionDropdown, fullscreenDropdown;
    [SerializeField] private TMP_InputField inputField;

    private void Start()
    {
        Screen.SetResolution(1366, 768, FullScreenMode.Windowed);
        inputField.text = "50";
        SetResolutionDropdownValues();
        SetFullScreenDropdownValues();
        gameObject.SetActive(false);
    }


    private void Update()
    {
    }

    private void SetFullScreenDropdownValues()
    {
        // fullscreen dropdown
        fullscreenDropdown.ClearOptions();
        fullscreenDropdown.AddOptions(
            new List<string>
            {
                "Windowed",
                "ExclusiveFullScreen",
                "FullScreenWindow",
                "MaximizedWindow"
            });
        fullscreenDropdown.value = 0;
        fullscreenDropdown.RefreshShownValue();
    }

    private void SetResolutionDropdownValues()
    {
        // fullscreen dropdown
        var options = new List<string>();
        var resolutions = Screen.resolutions;
        var currentResIndex = 0;
        for (var i = 0; i < resolutions.Length; i++)
        {
            var width = resolutions[i].width;
            var height = resolutions[i].height;
            var refreshRate = resolutions[i].refreshRateRatio;
            if (Screen.width == width && Screen.height == height) currentResIndex = i;
            options.Add($"{width}x{height}px - {refreshRate}Hz");
        }

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResIndex;
        resolutionDropdown.RefreshShownValue();
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
            var clampedSens = Mathf.Clamp(inputtedSens, .1f, 100);
            return clampedSens;
        }

        inputField.text = inputtedSens.ToString();
        return inputtedSens;
    }


    public void OnInputFieldChanged()
    {
        ValidateInputFieldValues();
    }

    public void OnFullScreenDropdownChanged()
    {
        // fullscreenDropdown.value
    }

    public void OnApplyButton()
    {
        var logs = new List<string>();
        logs.Add("OnApplyButton");
        // handle mouse sens
        var sens = ValidateInputFieldValues();
        GameStateMachine.Instance.SetAllCameraSens(sens);
        logs.Add("Applied: sens " + sens);


        //TODO handle screen res

        var screenRes = Screen.resolutions[resolutionDropdown.value];
        var fullScreenMode = fullscreenDropdown.value switch
        {
            0 => FullScreenMode.Windowed,
            1 => FullScreenMode.ExclusiveFullScreen,
            2 => FullScreenMode.FullScreenWindow,
            3 => FullScreenMode.MaximizedWindow,
            _ => FullScreenMode.Windowed
        };

        Screen.SetResolution(screenRes.width, screenRes.height, fullScreenMode);
        logs.Add("Applied: screenRes " + screenRes);
        logs.Add("Applied: fullScreenMode " + fullScreenMode);
        logs.Add("Applied: fullScreenMode " + fullscreenDropdown.value);


        Debug.Log(string.Join("\n", logs));
    }
}