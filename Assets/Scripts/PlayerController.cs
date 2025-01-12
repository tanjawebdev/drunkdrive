using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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

    private Rigidbody rb;
    //private AudioSource audioSource;

    //private int count = 0;
    //private int maxCount = 0;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //audioSource = GetComponent<AudioSource>();

        //maxCount = GameObject.FindGameObjectsWithTag("Diamond").Length;
        //SetCountText();
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
