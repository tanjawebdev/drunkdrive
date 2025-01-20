using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using extOSC;

public class Menu : MonoBehaviour
{
    public TextMeshProUGUI blinkingtext; // For TextMeshPro
    // public UnityEngine.UI.Text text; // Use this if you're using regular UI Text

    [SerializeField] private OSCReceiver _oscReceiver; // OSC Receiver for ZigSim

    public float blinkInterval = 0.5f; // Time between blinks
    public Animator animator; // Reference to the Animator
    public string stateName;  // Name of the animation state in Animator
    public float interval = 4f; // Time in seconds between triggers
    public AudioSource drinking_sound;
    private float bottle_tilt = -1f;

    private void Start()
    {
        if (blinkingtext == null)
        {
            blinkingtext = GetComponent<TextMeshProUGUI>(); // Automatically assign if not set
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (drinking_sound == null){
            drinking_sound = GetComponent<AudioSource>();
        }

        StartCoroutine(BlinkText());

        InvokeRepeating(nameof(TriggerAnimation), interval, interval);
         _oscReceiver.Bind("/ZIGSIM/beer/gravity", HandleGravityMessage);
    }

    private void TriggerAnimation(){
        animator.Play(stateName, 0, 0f);
        StartCoroutine(PlaySound());
    }

    private IEnumerator PlaySound(){
        yield return new WaitForSeconds(1f);
        drinking_sound.Play();
    }

    private IEnumerator BlinkText()
    {
        while (true)
        {
            blinkingtext.enabled = !blinkingtext.enabled; // Toggle visibility
            yield return new WaitForSeconds(blinkInterval);
        }
    }

    public void OnQuitButton() {
        Application.Quit();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.P) || bottle_tilt >= -0.5f){
            SceneManager.LoadScene(1);
        }
    }

    private void HandleGravityMessage(OSCMessage message)
    {
        bottle_tilt = message.Values[1].FloatValue;
    }
}
