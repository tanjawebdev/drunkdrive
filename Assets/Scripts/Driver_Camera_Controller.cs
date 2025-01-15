using UnityEngine;

public class DriverHeadMotion : MonoBehaviour
{
    [Header("Bump Settings")]
    public float bumpIntensity = 0.1f; // Intensity of bumps
    public float bumpFrequency = 5f;  // How frequently bumps occur

    [Header("Momentum Settings")]
    public float momentumSmoothness = 0.1f; // Smoothness of head movement
    public float momentumDamping = 0.9f;    // Damping effect on the camera's motion

    [Header("Noise Settings")]
    public bool usePerlinNoise = true;       // Use Perlin noise for natural bumps
    public float noiseScale = 1f;           // Scale of Perlin noise
    public float noiseSpeed = 1f;           // Speed of Perlin noise animation

    private Vector3 initialPosition;        // Initial camera position
    private Vector3 velocity = Vector3.zero; // Simulated velocity for momentum
    private float timeOffset;               // Offset for Perlin noise

    void Start()
    {
        // Save the initial position of the camera
        initialPosition = transform.localPosition;

        // Randomize the Perlin noise time offset
        timeOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        // Simulate bumps
        Vector3 bumpOffset = Vector3.zero;

        if (usePerlinNoise)
        {
            // Use Perlin noise for natural bumping
            float noiseX = Mathf.PerlinNoise(timeOffset, Time.time * noiseSpeed) - 0.5f;
            float noiseY = Mathf.PerlinNoise(timeOffset + 1, Time.time * noiseSpeed) - 0.5f;
            bumpOffset = new Vector3(noiseX, noiseY, 0f) * bumpIntensity;
        }
        else
        {
            // Sinusoidal bumps for simpler motion
            bumpOffset = new Vector3(
                Mathf.Sin(Time.time * bumpFrequency) * bumpIntensity,
                Mathf.Cos(Time.time * bumpFrequency) * bumpIntensity,
                0f
            );
        }

        // Simulate head momentum (damping + smooth following)
        Vector3 targetPosition = initialPosition + bumpOffset;
        velocity = Vector3.Lerp(velocity, (targetPosition - transform.localPosition), momentumSmoothness);
        velocity *= momentumDamping;

        // Apply the resulting position
        transform.localPosition += velocity;
    }
}