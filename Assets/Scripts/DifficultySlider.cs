using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpeedSettingManager : MonoBehaviour
{
    public Slider speedSlider; // Assign the slider in the Inspector
    public TextMeshProUGUI speedValueText; // Assign the text to display the value

    private void Start()
    {
        // Set slider's default value
        speedSlider.value = PlayerPrefs.GetInt("MaxSpeed", 50); // Default max speed is 50
        UpdateSpeedText(speedSlider.value);

        // Add a listener to the slider's value change event
        speedSlider.onValueChanged.AddListener(delegate { OnSliderValueChanged(); });
    }

    private void OnSliderValueChanged()
    {
        // Update the displayed value
        UpdateSpeedText(speedSlider.value);

        // Save the current value using PlayerPrefs
        PlayerPrefs.SetInt("MaxSpeed", Mathf.RoundToInt(speedSlider.value));
    }

    private void UpdateSpeedText(float value)
    {
        speedValueText.text = value.ToString("F0");
    }
}