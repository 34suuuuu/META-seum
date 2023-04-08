using System;
using System.Collections.Generic;
using Michsky.UI.ModernUIPack;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionManager : MonoBehaviour
{
    private FullScreenMode _screenMode;
    public CustomDropdown resolutionDropdown;
    public CustomToggle fullScreenBtn;
    private List<Resolution> _resolutions = new List<Resolution>();
    private int _resolutionNum;

    void Start()
    {
        InitUI();
    }

    void InitUI()
    {
        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            Debug.Log(Screen.resolutions[i].refreshRate);
            if (Screen.resolutions[i].refreshRate == 60 || Screen.resolutions[i].refreshRate == 120)
                _resolutions.Add(Screen.resolutions[i]);
        }

        resolutionDropdown.dropdownItems.Clear();

        int optionNum = 0;
        foreach (Resolution item in _resolutions)
        {
            string resolution = item.width + "x" + item.height + " " + item.refreshRate + "hz";
            resolutionDropdown.CreateNewItem(resolution, null);

            if (item.width == Screen.width && item.height == Screen.height)
                resolutionDropdown.selectedItemIndex = optionNum;
            optionNum++;
        }

        resolutionDropdown.SetupDropdown();

        resolutionDropdown.dropdownEvent.AddListener(DropdownChange);

        fullScreenBtn.toggleObject.isOn = Screen.fullScreenMode.Equals(FullScreenMode.FullScreenWindow);
    }

    void DropdownChange(int x)
    {
        _resolutionNum = x;
        Debug.Log("Changed to " + x.ToString());
    }

    public void FullScreenBtn(bool isFull)
    {
        _screenMode = isFull ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
    }

    public void OnClickResolutionApplyBtn()
    {
        Resolution resolution = _resolutions[_resolutionNum];
        Screen.SetResolution(resolution.width, resolution.height, _screenMode);
    }
}