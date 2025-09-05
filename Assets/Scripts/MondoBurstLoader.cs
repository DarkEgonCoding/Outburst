using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MondoBurstLoader : MonoBehaviour
{
    public static OutburstDatabase mondoDatabase;

    static MondoBurstLoader()
    {
        LoadJSON();
    }

    public static void LoadJSON()
    {
        TextAsset jsonText = Resources.Load<TextAsset>("MondoBurst");
        if (jsonText != null)
        {
            mondoDatabase = JsonUtility.FromJson<OutburstDatabase>(jsonText.text);
            Debug.Log($"Loaded {mondoDatabase.cards.Count} questions.");
        }
        else
        {
            Debug.LogError("Could not load JSON!");
        }
    }

    public static OutburstQuestion GetRandomQuestion()
    {
        if (mondoDatabase == null || mondoDatabase.cards == null || mondoDatabase.cards.Count == 0)
        {
            Debug.LogError("Outburst database is empty!");
            return null;
        }

        int randomIndex = Random.Range(0, mondoDatabase.cards.Count);
        return mondoDatabase.cards[randomIndex];
    }
}
