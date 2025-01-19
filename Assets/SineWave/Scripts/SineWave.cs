using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using extOSC;
using System;
using System.Runtime.CompilerServices;

public class SineWave : MonoBehaviour
{
    #region Variables
    [SerializeField] private OSCReceiver _oscReceiver; // OSC Receiver for ZigSim

    // Shader and Material
    public Shader SineWaveShader;
    private Material mat;

    //reference drunk level text
    public Text Drunk_Level_Text;

    // Drunkenness variables
    public float drunk_level = 1f; // Current drunk level
    public float target_drunk_level = 1f; // Target drunk level
    private float drunkPhase = 0f; // Phase for oscillation
    public float drunkDecayRate = 0.2f; // How quickly the drunk level decreases over time
    public float drunkBumpAmount = 1f; // How much to increase the target drunk level when P is pressed
    public float drunkMinLevel = 0f; // Minimum value for drunk_level
    public float drunkIncreaseSpeed = 2f; // Speed at which drunk_level interpolates to target_drunk_level
    public float drunkMaxLevel = 3f; // or whatever upper limit you want
    private float bottleAngle = 0f;
    private float bottle_tilt = -1f;
    public Slider drunk_slider;
    public Image fillImage;
    public Image sliderbackground;
    public Color flashColor = Color.red;
    public float flashSpeed = 2f;
    private float timeAtZeroDrunkness = 0f;
    private bool isFlashing = false;
    private bool DrunknessLocked = false;
    public AudioSource sober_regret_sound;
    private bool sober_gameover_finished = false;

    private bool _XAxis;
    public bool XAxis
    {
        get { return _XAxis; }
        set
        {
            _XAxis = value;
            float X = (XAxis) ? 1f : 0f;
            mat.SetFloat("_XAxis", X);
        }
    }

    private float _HorizontalOffset;
    public float HorizontalOffset
    {
        get { return _HorizontalOffset; }
        set
        {
            _HorizontalOffset = value;
            mat.SetFloat("_HorizontalOffset", HorizontalOffset);
        }
    }

    private float _VerticalOffset;
    public float VerticalOffset
    {
        get { return _VerticalOffset; }
        set
        {
            _VerticalOffset = value;
            mat.SetFloat("_VerticalOffset", VerticalOffset);
        }
    }

    private float _Amplitude;
    public float Amplitude
    {
        get { return _Amplitude; }
        set
        {
            _Amplitude = value;
            mat.SetFloat("_Amplitude", _Amplitude);
        }
    }

    private float _Frequency;
    public float Frequency
    {
        get { return _Frequency; }
        set
        {
            _Frequency = value;
            mat.SetFloat("_Frequency", _Frequency);
        }
    }

    // Speed of oscillation, will scale with drunk_level
    public float baseOscillationSpeed = 1f;
    public float oscillationSpeedMultiplier = 0.5f; // How much the speed increases with drunk level

    #endregion

    #region Methods

    void Start()
    {
        mat = new Material(SineWaveShader);

        // Default values
        XAxis = false; // Set to true if you want the wave along the X-axis
        HorizontalOffset = 0f;
        VerticalOffset = 0f;
        Amplitude = 0.1f;
        Frequency = 25f;

        _oscReceiver.Bind("/ZIGSIM/beer/gravity", HandleGravityMessage);
    }

    void Update()
    {
        if(DrunknessLocked){
            drunk_level = 0;
        }
        else {
            // Handle input to increase the target drunk_level
            if (Input.GetKeyDown(KeyCode.P))
            {
                target_drunk_level += drunkBumpAmount;
                target_drunk_level = Mathf.Clamp(target_drunk_level, drunkMinLevel, drunkMaxLevel);
            }

            if (Input.GetKeyDown(KeyCode.M)){
                sober_regret_sound.Play();
            }

            if (bottle_tilt >= -0.5f)
            {
                target_drunk_level += drunkBumpAmount * 0.01f;
                target_drunk_level = Mathf.Clamp(target_drunk_level, drunkMinLevel, drunkMaxLevel);
            }
        }

        // Gradually interpolate drunk_level toward target_drunk_level
        drunk_level = Mathf.Lerp(drunk_level, target_drunk_level, Time.deltaTime * drunkIncreaseSpeed);

        drunk_level = Mathf.Clamp(drunk_level, drunkMinLevel, drunkMaxLevel);

        // Gradually decrease the target drunk_level over time
        target_drunk_level = Mathf.Max(target_drunk_level - (drunkDecayRate * Time.deltaTime), drunkMinLevel);

        // Adjust oscillation speed based on drunk_level
        float currentOscillationSpeed = baseOscillationSpeed + drunk_level * oscillationSpeedMultiplier;

        // Increment the phase for oscillation
        drunkPhase += Time.deltaTime * currentOscillationSpeed;

        // Oscillate the HorizontalOffset to make the wave "wobble"
        VerticalOffset = Mathf.Sin(drunkPhase) * drunk_level;

        // Oscillate the Amplitude to simulate "drunken" distortion
        Amplitude = Mathf.Sin(drunkPhase * 0.5f) * drunk_level * 0.1f;

        // Optional: Add a subtle vertical wobble for extra effect
        VerticalOffset = Mathf.Cos(drunkPhase * 0.7f) * drunk_level * 0.05f;

        Drunk_Level_Text.text = drunk_level.ToString("F2");

        //Handle Drunk_Slider
        if (drunk_slider != null)
        {
            drunk_slider.value = drunk_level;

            // Update the bar's color dynamically
            if (fillImage != null)
            {
                float t = drunk_level / drunkMaxLevel; // Normalize drunkness level
                fillImage.color = Color.Lerp(Color.red, Color.green, t);
            }

            // Handle flashing when drunkness is 0
            if (drunk_level <= 0.1)
            {
                if (!isFlashing)
                {
                    isFlashing = true;
                    timeAtZeroDrunkness = 0f; // Reset timer
                }

                FlashSlider();

                // Increment timer
                timeAtZeroDrunkness += Time.deltaTime;

                // Check if it has been at 0 for more than 5 seconds
                if (timeAtZeroDrunkness > 5f)
                {
                    Brakes();
                    DrunknessLocked = true;
                }
            }
            else
            {
                isFlashing = false;
                timeAtZeroDrunkness = 0f; // Reset timer
                if (sliderbackground != null)
                {
                    sliderbackground.color = new Color(85/255f, 51/255f, 0f, 0f); // Reset color to default (e.g., green)
                }
            }
        }
    }

    // Flash the slider fill color
    private void FlashSlider()
    {
        if (sliderbackground != null)
        {
            float t = Mathf.Abs(Mathf.Sin(Time.time * flashSpeed)); // Calculate flashing effect
            sliderbackground.color = Color.Lerp(new Color(85/255f, 51/255f, 0f, 0f), flashColor, t);
        }
    }

    // Set the target speed in the PlayerController script
    private void Brakes()
    {
        PrometeoCarController prometeoController = GetComponentInParent<PrometeoCarController>();
        PlayerController playerController = GetComponentInParent<PlayerController>();
        if (prometeoController != null)
        {
            prometeoController.Brakes();
            
            if(prometeoController.carSpeed <= 0.1f){
                playerController.winLooseText.text = "You sobered up";
                playerController.winLooseText.color = Color.red;
                StartCoroutine(FadeOutAudio(prometeoController.carEngineSound,50f)); // Fades out over 2 seconds
                if (!sober_gameover_finished){
                    sober_regret_sound.Play();
                    Debug.Log("Sober Gameover");
                    sober_gameover_finished = true;
                }
            }
        }
    }

    private IEnumerator FadeOutAudio(AudioSource audioSource, float duration)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0.3)
        {
            audioSource.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }
    }

    private void HandleGravityMessage(OSCMessage message)
    {
        bottle_tilt = message.Values[1].FloatValue;
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (mat == null)
        {
            return;
        }

        Graphics.Blit(src, dest, mat);
    }

    #endregion
}