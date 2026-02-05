using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class FullScreen : MonoBehaviour
{
    public Toggle toggle;
    public TMP_Dropdown resolutionsDropDown;

    private Resolution[] resolutions;

    void Start()
    {
        SetupResolutions();

        // Cargar estado de Pantalla Completa
        bool isFull = PlayerPrefs.GetInt("IsFullScreen", 1) == 1;
        Screen.fullScreen = isFull;
        if (toggle != null) toggle.isOn = isFull;
    }

    public void SetupResolutions()
    {
        Resolution[] allResolutions = Screen.resolutions;
        resolutionsDropDown.ClearOptions();

        List<Resolution> uniqueResolutions = new List<Resolution>();
        List<string> options = new List<string>();

        for (int i = 0; i < allResolutions.Length; i++)
        {
            bool exists = false;
            for (int j = 0; j < uniqueResolutions.Count; j++)
            {
                if (uniqueResolutions[j].width == allResolutions[i].width &&
                    uniqueResolutions[j].height == allResolutions[i].height)
                {
                    if (allResolutions[i].refreshRateRatio.value > uniqueResolutions[j].refreshRateRatio.value)
                    {
                        uniqueResolutions[j] = allResolutions[i];
                    }
                    exists = true;
                    break;
                }
            }

            if (!exists)
            {
                uniqueResolutions.Add(allResolutions[i]);
            }
        }

        resolutions = uniqueResolutions.ToArray();
        int currentResIndex = 0;
        int savedIndex = PlayerPrefs.GetInt("resolutionIndex", -1);

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (savedIndex == -1)
            {
                if (resolutions[i].width == Screen.currentResolution.width &&
                    resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResIndex = i;
                }
            }
        }

        resolutionsDropDown.AddOptions(options);
        int finalIndex = (savedIndex != -1) ? savedIndex : currentResIndex;

        resolutionsDropDown.value = Mathf.Clamp(finalIndex, 0, resolutions.Length - 1);
        resolutionsDropDown.RefreshShownValue();
    }

    public void SetFullScreen(bool isFull)
    {
        Screen.fullScreen = isFull;
        PlayerPrefs.SetInt("IsFullScreen", isFull ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void ChangeResolution(int index)
    {
        Resolution resolution = resolutions[index];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode, resolution.refreshRateRatio);

        PlayerPrefs.SetInt("resolutionIndex", index);
        PlayerPrefs.Save();
    }
}