using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OutburstLoader
{
    public static OutburstDatabase database;

    static OutburstLoader()
    {
        LoadJSON();
    }

    public static void LoadJSON()
    {
        TextAsset jsonText = Resources.Load<TextAsset>("OutburstQuestions");
        if (jsonText != null)
        {
            database = JsonUtility.FromJson<OutburstDatabase>(jsonText.text);
            Debug.Log($"Loaded {database.cards.Count} questions.");
        }
        else
        {
            Debug.LogError("Could not load JSON!");
        }
    }

    public static OutburstQuestion GetRandomQuestion()
    {
        if (database == null || database.cards == null || database.cards.Count == 0)
        {
            Debug.LogError("Outburst database is empty!");
            return null;
        }

        int randomIndex = Random.Range(0, database.cards.Count);
        return database.cards[randomIndex];
    }
}
