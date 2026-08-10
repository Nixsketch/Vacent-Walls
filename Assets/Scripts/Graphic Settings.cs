using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    public TMP_Dropdown qualityDropdown;

    void Start()
    {
        // Clear default options in the inspector
        qualityDropdown.ClearOptions();

        // Create a list from the built-in names array (includes "Potato")
        List<string> options = new List<string>(QualitySettings.names);
        
        // Add options to the dropdown
        qualityDropdown.AddOptions(options);

        // Set the current value to match the active quality level
        qualityDropdown.value = QualitySettings.GetQualityLevel();
        
        // Register the listener to apply changes
        qualityDropdown.onValueChanged.AddListener(delegate {SetQuality(qualityDropdown.value); });

         // Force "Potato" (Index 0) temporarily to test
    QualitySettings.SetQualityLevel(0); 
    
    // Check what Unity thinks is active
    Debug.Log("Current Quality Index: " + QualitySettings.GetQualityLevel()); 
    Debug.Log("Current Quality Name: " + QualitySettings.names[QualitySettings.GetQualityLevel()]);
}

        public void SetQuality(int qualityIndex)
    {
        
        QualitySettings.SetQualityLevel(qualityIndex);
        // Optional: Save to PlayerPrefs here
        PlayerPrefs.SetInt("QualitySetting", qualityIndex);
    }
}   