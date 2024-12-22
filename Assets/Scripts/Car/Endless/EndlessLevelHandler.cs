using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessLevelHandler : MonoBehaviour
{
    [SerializeField]
    GameObject[] sectionsPrefabs;

    GameObject[] sectionsPool = new GameObject[20];
    GameObject[] sections = new GameObject[10];

    Transform playerCarTransform;

    WaitForSeconds waitFor100ms = new WaitForSeconds(0.1f);

    const float sectionLength = 26;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start() method called in EndlessLevelHandler.");

        // Find the player transform
        playerCarTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (playerCarTransform == null)
        {
            Debug.LogError("Player GameObject with tag 'Player' not found. Please assign the 'Player' tag to the appropriate GameObject.");
            return;
        }
        Debug.Log("playerCarTransform assigned successfully.");

        // Initialize the pool
        Debug.Log("Initializing sections pool...");
        int prefabIndex = 0;
        for (int i = 0; i < sectionsPool.Length; i++) // Fixed the loop condition
        {
            sectionsPool[i] = Instantiate(sectionsPrefabs[prefabIndex]);
            if (sectionsPool[i] == null)
            {
                Debug.LogError($"Failed to instantiate section prefab at index {prefabIndex}.");
                continue;
            }
            sectionsPool[i].SetActive(false);
            Debug.Log($"Section prefab {prefabIndex} instantiated and added to pool.");

            prefabIndex++;
            if (prefabIndex >= sectionsPrefabs.Length)
                prefabIndex = 0;
        }

        // Add the first sections to the road
        Debug.Log("Placing initial sections...");
        for (int i = 0; i < sections.Length; i++)
        {
            GameObject randomSection = GetRandomSectionFromPool();
            if (randomSection == null)
            {
                Debug.LogError("Failed to get a random section from the pool during initialization.");
                continue;
            }

            randomSection.transform.position = new Vector3(0, 0, i * sectionLength);
            randomSection.SetActive(true);
            sections[i] = randomSection;
            Debug.Log($"Section placed at position {randomSection.transform.position}.");
        }

        Debug.Log("Starting UpdateLessOften coroutine...");
        StartCoroutine(UpdateLessOftenCO());
    }

    IEnumerator UpdateLessOftenCO()
    {
        while (true)
        {
            UpdateSectionPositions();
            yield return waitFor100ms;
        }
    }

    void UpdateSectionPositions()
    {
        for (int i = 0; i < sections.Length; i++)
        {
            if (sections[i] == null)
            {
                Debug.LogWarning($"Section at index {i} is null. Skipping...");
                continue;
            }

            if (playerCarTransform == null)
            {
                Debug.LogError("playerCarTransform is null during UpdateSectionPositions. Cannot proceed.");
                return;
            }

            if (sections[i].transform.position.z - playerCarTransform.position.z < -sectionLength)
            {
                Vector3 lastSectionPosition = sections[i].transform.position;
                sections[i].SetActive(false);

                sections[i] = GetRandomSectionFromPool();
                if (sections[i] == null)
                {
                    Debug.LogError("Failed to get a new section from the pool during UpdateSectionPositions.");
                    continue;
                }

                sections[i].transform.position = new Vector3(lastSectionPosition.x, 0, lastSectionPosition.z + sectionLength * sections.Length);
                sections[i].SetActive(true);
                Debug.Log($"Section moved to new position {sections[i].transform.position}.");
            }
        }
    }

    GameObject GetRandomSectionFromPool()
    {
        int randomIndex = Random.Range(0, sectionsPool.Length);
        bool isNewSectionFound = false;

        while (!isNewSectionFound)
        {
            if (!sectionsPool[randomIndex].activeInHierarchy)
            {
                isNewSectionFound = true;
            }
            else
            {
                randomIndex++;
                if (randomIndex >= sectionsPool.Length)
                    randomIndex = 0;
            }
        }

        if (sectionsPool[randomIndex] == null)
        {
            Debug.LogError($"Section at index {randomIndex} in the pool is null.");
        }
        return sectionsPool[randomIndex];
    }
}