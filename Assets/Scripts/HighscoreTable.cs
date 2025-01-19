using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public class HighscoreTable : MonoBehaviour
{
    public static HighscoreTable Instance; // For easy global access
    private string savePath;
    public TextMeshProUGUI Rank_Text;
    public TextMeshProUGUI Scores_Text;
    public TextMeshProUGUI GameId_Text;
    private ScoreData scoreData = new ScoreData();

    void Awake()
    {
        // Decide on a path, for example in persistentDataPath
        savePath = Path.Combine(Application.persistentDataPath, "scores.json");

        // Load existing data if file exists
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            scoreData = JsonUtility.FromJson<ScoreData>(json);
            if (scoreData == null) {
                scoreData = new ScoreData(); // fallback if JSON was empty/corrupted
            }
        }
    }

    public void AddHighscoreEntry(int gameId, float averageDrunkness)
    {
        // Create a new entry
        ScoreEntry newEntry = new ScoreEntry {
            gameID = gameId,
            averageDrunkness = averageDrunkness
        };

        // Add it to the list
        scoreData.scores.Add(newEntry);

        // (Optional) sort by drunkness or some other metric
        // scoreData.scores.Sort((x, y) => x.averageDrunkness.CompareTo(y.averageDrunkness));

        // Save the updated data
        SaveData();
        DisplayTop5();
    }

    public void ClearScores()
    {
        scoreData = new ScoreData();  // brand new, empty list
        SaveData();
        Debug.Log("High score data cleared!");
    }

    private void SaveData()
    {
        string json = JsonUtility.ToJson(scoreData, true);
        File.WriteAllText(savePath, json);
    }

    public void DisplayTop5()
    {
        // Sort descending by averageDrunkness
        scoreData.scores.Sort((a, b) => b.averageDrunkness.CompareTo(a.averageDrunkness));

        // Prepare strings
        string rankString = "";
        string scoresString = "";
        string gameIdString = "";

        // We only want up to 5 entries
        int count = Mathf.Min(scoreData.scores.Count, 5);

        for (int i = 0; i < count; i++)
        {
            // Some quick rank label: 1st, 2nd, 3rd, 4th, 5th
            string rankLabel = "";
            switch (i)
            {
                case 0: rankLabel = "1st"; break;
                case 1: rankLabel = "2nd"; break;
                case 2: rankLabel = "3rd"; break;
                case 3: rankLabel = "4th"; break;
                case 4: rankLabel = "5th"; break;
            }

            rankString += rankLabel + "\n";
            scoresString += scoreData.scores[i].averageDrunkness.ToString("F2") + "\n";
            gameIdString += scoreData.scores[i].gameID + "\n";
        }
        // Logs have to stay for unknown reasons
        if (Rank_Text == null) Debug.LogError("Rank_Text is NULL!");
        if (Scores_Text == null) Debug.LogError("Scores_Text is NULL!");
        if (GameId_Text == null) Debug.LogError("GameId_Text is NULL!");

        // Assign to TextMeshPro fields
        Rank_Text.text = rankString;
        Scores_Text.text = scoresString;
        GameId_Text.text = gameIdString;
    }
}

[System.Serializable]
public class ScoreData
{
    public List<ScoreEntry> scores = new List<ScoreEntry>();
}

[System.Serializable]
public class ScoreEntry
{
    public int gameID;
    public float averageDrunkness;
}