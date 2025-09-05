using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager instance;
    public List<LeaderboardSlot> slots;

    [Header("UI References")]
    [SerializeField] private List<TextMeshProUGUI> nameTexts;
    [SerializeField] private List<TextMeshProUGUI> scoreTexts;

    private const int MAX_SLOTS = 5;
    private const string SaveKey = "Leaderboard";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            LoadLeaderboard();
            UpdateUI();
        }

    }

    public void HandleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SaveLeaderboard();
            GameController.instance.OpenMenu();
        }
    }

    public void TryAddNewScore(string playerName, int score)
    {
        slots.Add(new LeaderboardSlot { playerName = playerName, score = score });

        slots.Sort((a, b) => b.score.CompareTo(a.score));

        // Keep only top 5
        if (slots.Count > MAX_SLOTS)
            slots = slots.GetRange(0, MAX_SLOTS);

        // Save
        SaveLeaderboard();
        UpdateUI();
    }

    private void SaveLeaderboard()
    {
        for (int i = 0; i < MAX_SLOTS; i++)
        {
            if (i < slots.Count)
            {
                PlayerPrefs.SetString(SaveKey + "_Name_" + i, slots[i].playerName);
                PlayerPrefs.SetInt(SaveKey + "_Score_" + i, slots[i].score);
            }
            else
            {
                PlayerPrefs.DeleteKey(SaveKey + "_Name_" + i);
                PlayerPrefs.DeleteKey(SaveKey + "_Score_" + i);
            }
        }

        PlayerPrefs.Save();
    }

    private void LoadLeaderboard()
    {
        slots.Clear();

        for (int i = 0; i < MAX_SLOTS; i++)
        {
            string nameKey = SaveKey + "_Name_" + i;
            string scoreKey = SaveKey + "_Score_" + i;

            if (PlayerPrefs.HasKey(nameKey) && PlayerPrefs.HasKey(scoreKey))
            {
                LeaderboardSlot slot = new LeaderboardSlot
                {
                    playerName = PlayerPrefs.GetString(nameKey),
                    score = PlayerPrefs.GetInt(scoreKey)
                };
                slots.Add(slot);
            }
        }
    }
    
    private void UpdateUI()
    {
        for (int i = 0; i < MAX_SLOTS; i++)
        {
            if (i < slots.Count)
            {
                nameTexts[i].text = slots[i].playerName;
                scoreTexts[i].text = slots[i].score.ToString();
            }
            else
            {
                nameTexts[i].text = "---";
                scoreTexts[i].text = "0";
            }
        }
    }
}

[System.Serializable]
public class LeaderboardSlot
{
    public int score;
    public string playerName;
}
