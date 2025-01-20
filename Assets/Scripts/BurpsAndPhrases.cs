using UnityEngine;
using System.Collections;

public class RandomAudioSourcePlayer : MonoBehaviour
{
    public AudioSource[] audioSources; // Array of AudioSources to play sounds

    private void Start()
    {
        // Start the coroutine to play sounds at random intervals
        StartCoroutine(PlayRandomAudioSource());
    }

    private IEnumerator PlayRandomAudioSource()
    {
        while (true)
        {
            // Wait for a random interval between 5 and 10 seconds
            float randomInterval = Random.Range(8f, 13f);
            yield return new WaitForSeconds(randomInterval);

            // Select a random AudioSource from the array
            if (audioSources.Length > 0)
            {
                int randomIndex = Random.Range(0, audioSources.Length);
                AudioSource randomAudioSource = audioSources[randomIndex];

                // Play the sound if it's not already playing
                if (!randomAudioSource.isPlaying)
                {
                    randomAudioSource.Play();
                }
            }
        }
    }
}