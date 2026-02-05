using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq; // Añadimos esto para poder ordenar fácilmente

public class FullScreen : MonoBehaviour
{
    public Toggle toggle;
    public TMP_Dropdown resolutionsDropDown;

    private Resolution[] resolutions;

    void Start()
    {
        SetupResolutions();

        bool isFull = PlayerPrefs.GetInt("IsFullScreen", 1) == 1;
        Screen.fullScreen = isFull;
        if (toggle != null) toggle.isOn = isFull;
    }

    public void SetupResolutions()
    {
        Resolution[] allResolutions = Screen.resolutions;
        resolutionsDropDown.ClearOptions();

        List<Resolution> filteredList = new List<Resolution>();

        // 1. Filtrar por 16:9
        for (int i = 0; i < allResolutions.Length; i++)
        {
            float aspectRatio = (float)allResolutions[i].width / allResolutions[i].height;
            if (Mathf.Abs(aspectRatio - (16f / 9f)) < 0.01f)
            {
                filteredList.Add(allResolutions[i]);
            }
        }

        // 2. Ordenar de mayor a menor (Ancho primero, luego Hz)
        // Usamos OrderByDescending para que las más altas salgan arriba
        List<Resolution> sortedList = filteredList
            .OrderByDescending(res => res.width)
            .ThenByDescending(res => res.refreshRateRatio.value)
            .ToList();

        // Si no hay 16:9, usamos todas (pero también las ordenamos)
        if (sortedList.Count == 0)
        {
            sortedList = allResolutions
                .OrderByDescending(res => res.width)
                .ThenByDescending(res => res.refreshRateRatio.value)
                .ToList();
        }

        resolutions = sortedList.ToArray();
        List<string> options = new List<string>();
        int currentResIndex = 0;
        int savedIndex = PlayerPrefs.GetInt("resolutionIndex", -1);

        // 3. Crear los textos para el Dropdown
        for (int i = 0; i < resolutions.Length; i++)
        {
            float refreshRate = (float)resolutions[i].refreshRateRatio.value;
            string option = resolutions[i].width + " x " + resolutions[i].height + " @ " + Mathf.Round(refreshRate) + "Hz";
            options.Add(option);

            // Intentar detectar la resolución actual si no hay nada guardado
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