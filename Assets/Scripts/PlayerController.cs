using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class PlayerController : MonoBehaviour
{
    public float speed = 1.0f;

    //public AudioClip collectSound;
    //public AudioClip deathSound;

    //public AudioSource backgroundMusic;
    //public TextMeshProUGUI countText;
    public TextMeshProUGUI winLooseText;

    public UnityEngine.UI.Image windshieldcrack;

    private Rigidbody rb;
    //private AudioSource audioSource;

    //private int count = 0;
    //private int maxCount = 0;

    public AudioSource crashSound; 
    public AudioSource carIdleSound;
    private PrometeoCarController carController;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //audioSource = GetComponent<AudioSource>();

        //maxCount = GameObject.FindGameObjectsWithTag("Diamond").Length;
        //SetCountText();

        // Get a reference to the PrometeoCarController
        carController = GetComponent<PrometeoCarController>();

        // Ensure the idle sound loops
        if (carIdleSound != null)
        {
            carIdleSound.loop = true;
        }

        if (windshieldcrack != null)
        {
            windshieldcrack.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
       

        if (other.gameObject.CompareTag("tree"))
        {
            //audioSource.PlayOneShot(deathSound);
            //backgroundMusic.Stop();
            rb.isKinematic = true;

            winLooseText.text = "Oh no! I ran into a tree and died.";
            winLooseText.color = Color.red;
            
            // Make the crash image visible
            if (windshieldcrack != null)
            {
                windshieldcrack.gameObject.SetActive(true);
            }

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
        }
    }

    private void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }
}
