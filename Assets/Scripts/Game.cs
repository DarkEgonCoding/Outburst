using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Game
{
    List<RoundType> GameRounds = new List<RoundType>() { };

    GameController gameController;

    public int score = 0;

    public RoundType CurrentRoundType 
    {
        get
        {
            if (currentRoundIndex >= 0 && currentRoundIndex < GameRounds.Count)
            {
                return GameRounds[currentRoundIndex];
            }
            return RoundType.None; // fallback if out of range
        }
    }

    public string playerName = "Unnamed";

    public int currentRoundIndex = -1;

    public void SetupRounds()
    {
        GameRounds = new List<RoundType>()
        {
            RoundType.OutburstGeneral,
            RoundType.OutburstGeneral,
            RoundType.MondoBurst
        };
    }

    public Game(string playerName, GameController gameController)
    {
        score = 0;
        currentRoundIndex = -1;
        SetupRounds();
        this.playerName = playerName;
        this.gameController = gameController;
    }

    public void StartNextRound()
    {
        currentRoundIndex++;

        if (currentRoundIndex >= GameRounds.Count)
        {
            Debug.Log("The game has ended!");
            LeaderboardManager.instance.TryAddNewScore(this.playerName, this.score);
            gameController.OpenLeaderboard();
            return;
        }

        RoundType round = GameRounds[currentRoundIndex];
        Debug.Log($"Starting round: {round}");
        gameController.OpenQuestionType(round);

        if (round == RoundType.OutburstGeneral)
        {
            OutburstQuestion question = GetRandomOutburstQuestion();

            if (question == null)
            {
                Debug.LogError("Outburst question is null! Check if the database is loaded correctly.");
                return;
            }
            Debug.Log($"Outburst Round: {question.title}");

            if (question == null) Debug.LogError("Question is null");

            gameController.LoadOutburstQuestionUI(question);

            OutburstInputManager.instance.StartRound(this, question);
        }
        else if (round == RoundType.MondoBurst)
        {
            OutburstQuestion question = GetRandomMondoQuestion();
            gameController.LoadMondoBurstUI(question);

            Debug.Log($"Outburst Round: {question.title}");
            if (question == null) Debug.LogError("Question is null");

            OutburstInputManager.instance.StartMondoBurst(this, question);
        }
    }

    public OutburstQuestion GetRandomOutburstQuestion()
    {
        if (OutburstLoader.database.cards.Count == 0)
        {
            Debug.Log("Database is null");
            return null;
        }

        int randomIndex = Random.Range(0, OutburstLoader.database.cards.Count);

        OutburstQuestion question = OutburstLoader.database.cards[randomIndex];
        Debug.Log($"Index: {randomIndex}");
        return question;
    }

    public OutburstQuestion GetRandomMondoQuestion()
    {
        if (MondoBurstLoader.mondoDatabase.cards.Count == 0)
        {
            Debug.Log("Database is null");
            return null;
        }

        int randomIndex = Random.Range(0, MondoBurstLoader.mondoDatabase.cards.Count);

        OutburstQuestion question = MondoBurstLoader.mondoDatabase.cards[randomIndex];
        Debug.Log($"Index: {randomIndex}");
        return question;
    }
}
