using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using extOSC;
using static UnityEngine.Rendering.DebugUI;


public class PlayerController : MonoBehaviour
{
    public float speed = 1.0f;

    //public AudioClip collectSound;
    //public AudioClip deathSound;

    //public AudioSource backgroundMusic;
    //public TextMeshProUGUI countText;
    public TextMeshProUGUI winLooseText;

    private Rigidbody rb;
    [SerializeField] private OSCReceiver _receiver;

    //private AudioSource audioSource;

    //private int count = 0;
    //private int maxCount = 0;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (_receiver != null)
        {
            _receiver.Bind("/ZIGSIM/tanjasPhone/compass", HandleMessage);
        }
        //audioSource = GetComponent<AudioSource>();

        //maxCount = GameObject.FindGameObjectsWithTag("Diamond").Length;
        //SetCountText();
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
       

        if (other.gameObject.CompareTag("tree"))
        {
            //audioSource.PlayOneShot(deathSound);
            //backgroundMusic.Stop();
            rb.isKinematic = true;

            winLooseText.text = "Oh no! I ran into a tree and died.";
            winLooseText.color = Color.red;

            //Invoke(nameof(BackToMenu), 5f);
        }
    }

    private void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }
}
