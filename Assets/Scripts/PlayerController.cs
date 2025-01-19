using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using extOSC;
using UnityEngine.SocialPlatforms.Impl;
using Unity.VisualScripting;
using UnityEngine.TestTools;

public class PlayerController : MonoBehaviour
{
    public float speed = 1.0f;

    public TextMeshProUGUI winLooseText;
    public TextMeshProUGUI playerScoreText;
    public GameObject finalOverlay;
    public GameObject carFire;
    public HighscoreTable highscoreTable;

    private Rigidbody rb;
    public AudioSource crashSound; 
    public AudioSource carIdleSound;
    private PrometeoCarController carController;
    [SerializeField] private OSCReceiver _receiver;

    public Transform cameraTransform; // Reference to the camera's Transform
    public Transform targetObject; // The object you want the camera to point at
    public float cameraMoveHeight = 20f; // The amount by which to move the camera up in the Y direction
    public float cameraMoveSpeed = 1f;   // Speed of the camera movement

    private Transform originalCameraParent; // Store the original parent of the camera
    private DriverHeadMotion driverHeadMotion; // Reference to the Driver Head Motion component
    private SineWave sineWave; // Reference to the SineWave component

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        carController = GetComponent<PrometeoCarController>();

        if (_receiver != null)
        {
            _receiver.Bind("/ZIGSIM/tanjasPhone/compass", HandleMessage);
        }

        // Store the original parent of the camera (which is the player)
        originalCameraParent = cameraTransform.parent;

        // Find the DriverHeadMotion component on the main camera (or assign it manually in the inspector)
        driverHeadMotion = cameraTransform.GetComponent<DriverHeadMotion>(); 

        // Find the SineWave component on the same object or camera (adjust as needed)
        sineWave = cameraTransform.GetComponent<SineWave>();
    }

    private void HandleMessage(OSCMessage message)
    {
        if (message.ToFloat(out float value))
        {
            Debug.Log($"Received value: {value}");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Collider-Death") || other.gameObject.CompareTag("Collider-Pushable"))
        {
            // Handle tree collision (optional code removed)
            rb.isKinematic = true;


            //Invoke(nameof(BackToMenu), 5f);

            if (crashSound != null)
            {
                crashSound.Play();
            }

            // Mute the car engine sound
            if (carController != null && carController.carEngineSound != null)
            {
                carController.carEngineSound.mute = true;
            }

            // Play the idle sound
            if (carIdleSound != null && !carIdleSound.isPlaying)
            {
                carIdleSound.Play();
            }

            highscoreTable.DisplayTop5();

            if (finalOverlay != null)
            {
                finalOverlay.SetActive(true);
            }

            if (carFire != null)
            {
                carFire.SetActive(true);
            }

            // Start coroutine to load the scene after a delay
            StartCoroutine(LoadSceneWithDelay(10f, 0));
        }

        if (other.gameObject.CompareTag("finishline"))
        {
            if (finalOverlay != null)
            {
                finalOverlay.SetActive(true);
            }
            StartCoroutine(MoveCameraToThirdPerson());
            winLooseText.text = "YOU WIN";
            winLooseText.color = Color.green;

            float averageDrunkness = sineWave.GetAverageDrunkness();
            playerScoreText.text = "Your average\npromille: " + averageDrunkness.ToString("F2");

            //Store in your high score / results
            GameIdManager gameIdManager = FindObjectOfType<GameIdManager>();

            if (gameIdManager != null)
            {
                int currentGameId = gameIdManager.GetCurrentGameId();
                // Add to high score results
                highscoreTable.AddHighscoreEntry(currentGameId, averageDrunkness);

                // or write to your own JSON file, etc.
            }
        }
    }

    private IEnumerator MoveCameraToThirdPerson()
    {
        // Disable the Driver Head Motion script when camera repositioning starts
        if (driverHeadMotion != null)
        {
            driverHeadMotion.enabled = false;
        }

        // Set the drunkDecayRate to 1f
        if (sineWave != null)
        {
            sineWave.target_drunk_level = 0f;
        }

        // Unparent the camera from the player to stop it from moving with the player
        cameraTransform.parent = null;  // Unparent the camera

        // Store initial position and target position for the camera
        Vector3 initialPosition = cameraTransform.position;
        Vector3 targetPosition = new Vector3(initialPosition.x, initialPosition.y + cameraMoveHeight, initialPosition.z);

        // Smoothly move the camera in the Y direction
        float timeElapsed = 0f;
        while (timeElapsed < 1f)
        {
            timeElapsed += Time.deltaTime * cameraMoveSpeed;  // Increment time based on the move speed
            cameraTransform.position = Vector3.Lerp(initialPosition, targetPosition, timeElapsed);

            // Make the camera point towards the target object
            cameraTransform.LookAt(targetObject);

            yield return null; // Wait until the next frame
        }

        // After the movement is complete, the camera is free, and it should no longer follow the player

        // Optionally, you can reparent the camera to the player again if needed (e.g., on some other event)
        // cameraTransform.parent = originalCameraParent;

        // Or just leave it unparented to keep it stationary at the new position
    }

    private void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }

    private IEnumerator LoadSceneWithDelay(float delay, int sceneIndex)
    {
        yield return new WaitForSeconds(delay); // Wait for the specified delay
        SceneManager.LoadScene(sceneIndex);     // Load the specified scene
    }
}