using UnityEngine;
using TMPro;

public class GameIdManager : MonoBehaviour
{
    private static int currentGameId;
    private const string GameIdPrefKey = "GameId";
    public TextMeshProUGUI Game_Id_Text;

    void Awake()
    {
        // Load the last saved GameID
        currentGameId = PlayerPrefs.GetInt(GameIdPrefKey, 0);

        // Increment at the start of this session
        currentGameId++;

        // Store the incremented value
        PlayerPrefs.SetInt(GameIdPrefKey, currentGameId);
        PlayerPrefs.Save();
    }

    void Start()
    {
        Game_Id_Text.text = "Game ID: " + currentGameId;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            PlayerPrefs.SetInt(GameIdPrefKey, 0);
            Game_Id_Text.text = "Game ID: 0";
            HighscoreTable table = FindObjectOfType<HighscoreTable>();
            if (table != null)
            {
                table.ClearScores();
            }
        }
    }

        // Provide public access to the current Game ID
        public int GetCurrentGameId()
    {
        return currentGameId;
    }
}