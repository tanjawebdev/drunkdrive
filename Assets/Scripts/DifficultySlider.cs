using TMPro;
using UnityEngine;
using UnityEngine.UI;
using extOSC;


public class SpeedSettingManager : MonoBehaviour
{
    public Slider speedSlider; // Assign the slider in the Inspector
    public TextMeshProUGUI speedValueText; // Assign the text to display the value
    [SerializeField] private OSCReceiver _receiver;
    private float initialCompassHeading;
    private float compassHeading;
    private bool initialCompassSet = false;

    private void Start()
    {
        if (_receiver == null)
        {
            _receiver = GetComponent<OSCReceiver>();
        }

        if (_receiver != null)
        {
            _receiver.Bind("/ZIGSIM/car/compass", HandleMessage);
            initialCompassHeading = compassHeading;
        }

        // Set slider's default value
        speedSlider.value = PlayerPrefs.GetInt("MaxSpeed", 50); // Default max speed is 50
        UpdateSpeedText(speedSlider.value);

        // Add a listener to the slider's value change event
        speedSlider.onValueChanged.AddListener(delegate { OnSliderValueChanged(); });
    }

    void Update()
    {
        float difficulty = compassHeading - initialCompassHeading + 50;
        float difficultyClamped = Mathf.Clamp(difficulty, 40, 100);
        UpdateSpeedText(difficultyClamped);
        UpdateSliderValue(difficultyClamped);
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

    private void UpdateSliderValue(float value)
    {
        speedSlider.value = value;
    }

    private void HandleMessage(OSCMessage message)
    {
        if (message.ToFloat(out float value))
        {
            compassHeading = value; // Update the current compass heading
            // Set initial compass heading only once
            if (!initialCompassSet)
            {
                initialCompassHeading = compassHeading;
                initialCompassSet = true; // Mark the flag as set
            }
        }
    }
}