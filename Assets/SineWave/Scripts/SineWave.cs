using UnityEngine;

public class SineWave : MonoBehaviour
{
    #region Variables

    // Shader and Material
    public Shader SineWaveShader;
    private Material mat;

    // Drunkenness variables
    public float drunk_level = 1f; // Current drunk level
    private float target_drunk_level = 1f; // Target drunk level
    private float drunkPhase = 0f; // Phase for oscillation
    public float drunkDecayRate = 0.1f; // How quickly the drunk level decreases over time
    public float drunkBumpAmount = 1f; // How much to increase the target drunk level when P is pressed
    public float drunkMinLevel = 0f; // Minimum value for drunk_level
    public float drunkIncreaseSpeed = 2f; // Speed at which drunk_level interpolates to target_drunk_level

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
        Frequency = 60f;
    }

    void Update()
    {
        // Handle input to increase the target drunk_level
        if (Input.GetKeyDown(KeyCode.P))
        {
            target_drunk_level += drunkBumpAmount;
        }

        // Gradually interpolate drunk_level toward target_drunk_level
        drunk_level = Mathf.Lerp(drunk_level, target_drunk_level, Time.deltaTime * drunkIncreaseSpeed);

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