using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public enum GameState { Menu, Game, Leaderboard, NamePick }

public enum RoundType { OutburstGeneral, ReverseBurst, ChallengeClock, SloppySeconds, MondoBurst, None }

public class GameController : MonoBehaviour
{
    [Header("Game Objects")]
    [SerializeField] public GameObject MainMenuObj;
    [SerializeField] public GameObject OutburstGameObj;
    [Header("Outburst General Details")]
    [SerializeField] public GameObject OutburstQuestionObj;
    [SerializeField] public TextMeshProUGUI OutburstQuestionTitle;
    [SerializeField] public List<TextMeshProUGUI> OutburstAnswersTxts;
    [SerializeField] public List<TextMeshProUGUI> InputLettersTxt;
    [Header("HUD")]
    [SerializeField] public TextMeshProUGUI ScoreTxt;
    [SerializeField] public TextMeshProUGUI PlayerNameTxt;
    [SerializeField] public GameObject CountdownObj;
    [SerializeField] public TextMeshProUGUI CountdownTxt;

    [Header("More Objects")]
    [SerializeField] public GameObject LeaderboardObj;
    [SerializeField] public GameObject PlayerNameObj;
    [SerializeField] GameObject LetterAnimationObj;

    [Header("Mondo Burst")]
    [SerializeField] public GameObject MondoBurstObj;
    [SerializeField] public TextMeshProUGUI MondoTitle;
    [SerializeField] private Transform mondoParent;
    [SerializeField] private TMP_Text mondoPrefab;

    [Header("Controllers and Managers")]
    [SerializeField] LetterAnimation letterAnimation;

    [SerializeField] MenuController menuController;
    [SerializeField] LeaderboardManager leaderboardManager;
    [SerializeField] NameManager nameManager;

    [Header("Colors")]
    [SerializeField] private Color missedAnswerColor;

    public static GameController instance;

    private GameState CurrentState = GameState.Menu;

    Game CurrentGame;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    private void Start()
    {
        CountdownObj.SetActive(false);
        OpenMenu();
    }

    private void Update()
    {
        if (CurrentState == GameState.Menu)
        {
            if (menuController != null) menuController.HandleUpdate();
        }
        else if (CurrentState == GameState.NamePick)
        {
            if (nameManager != null) nameManager.HandleUpdate();
        }
        else if (CurrentState == GameState.Leaderboard)
        {
            if (LeaderboardManager.instance != null) LeaderboardManager.instance.HandleUpdate();
        }
    }

    public void StartNewGame()
    {
        MainMenuObj.SetActive(false);
        OutburstGameObj.SetActive(false);
        LeaderboardObj.SetActive(false);
        PlayerNameObj.SetActive(true);
        CurrentState = GameState.NamePick;
    }

    public void SetupGame(string PlayerName)
    {
        PlayerNameObj.SetActive(false);
        OutburstGameObj.SetActive(true);
        LeaderboardObj.SetActive(false);
        MainMenuObj.SetActive(false);

        Debug.Log("Starting new game...");
        CurrentGame = new Game(PlayerName, this);
        CurrentState = GameState.Game;
        ScoreTxt.text = "0";
        PlayerNameTxt.text = PlayerName;
        CurrentGame.StartNextRound();
    }

    public void ResumeGame()
    {
        if (CurrentGame != null)
        {
            Debug.Log("Resuming game...");
        }
        else
        {
            Debug.LogWarning("No game to resume!");
        }
    }

    public void UpdateInputLettersTxt(List<string> chars)
    {
        for (int i = 0; i < InputLettersTxt.Count; i++)
        {
            if (i < chars.Count && !string.IsNullOrEmpty(chars[i]))
            {
                chars[i].ToUpper();
                InputLettersTxt[i].text = chars[i];
            }
            else
            {
                InputLettersTxt[i].text = "";
            }
        }
    }

    public void ShowAnswer(string answer)
    {
        foreach (var AnswerText in OutburstAnswersTxts)
        {
            if (AnswerText.text == answer)
            {
                AnswerText.alpha = 1;
            }
        }
    }

    public IEnumerator ShowIncompleteAnswers()
    {
        yield return new WaitForSeconds(2f);

        for (int i = 0; i < OutburstAnswersTxts.Count; i++)
        {
            // If you got the answer correct, skip
            if (OutburstAnswersTxts[i].alpha == 1) continue;

            var answertxt = OutburstAnswersTxts[i];
            answertxt.alpha = 1;
            answertxt.color = missedAnswerColor;
            Vector3 originalScale = answertxt.transform.localScale;
            Vector3 bigScale = originalScale * 1.3f; // 30% bigger

            // Scale up
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime;
                answertxt.transform.localScale = Vector3.Lerp(originalScale, bigScale, t / 0.2f);
                yield return null;
            }

            // Scale back down
            t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime;
                answertxt.transform.localScale = Vector3.Lerp(bigScale, originalScale, t / 0.2f);
                yield return null;
            }

            yield return new WaitForSeconds(1f);
        }
    }

    public IEnumerator UpdateGameScoreAnimation(Game game, int roundScore)
    {
        int currScore = game.score;
        int finalScore = currScore + roundScore;

        float delay = 0.3f;
        int increment = 1;   // score increment per step

        while (currScore < finalScore)
        {
            currScore += increment;

            if (currScore > finalScore)
                currScore = finalScore;

            game.score = currScore;

            ScoreTxt.text = game.score.ToString();
            yield return new WaitForSeconds(delay);
        }

        Debug.Log("Updating Scores Done!");
    }

    public void OpenQuestionType(RoundType round)
    {
        OutburstQuestionObj.SetActive(false);
        MondoBurstObj.SetActive(false);

        if (round == RoundType.OutburstGeneral) OutburstQuestionObj.SetActive(true);
        else if (round == RoundType.MondoBurst)
        {
            MondoBurstObj.SetActive(true);
            mondoParent.gameObject.SetActive(true);
        }
    }

    public void LoadOutburstQuestionUI(OutburstQuestion question)
    {
        List<string> items = question.items;

        OutburstQuestionTitle.text = question.title;
        OutburstQuestionTitle.alpha = 0;

        for (int i = 0; i < OutburstAnswersTxts.Count; i++)
        {
            if (items[i] == null)
            {
                Debug.LogWarning($"Items at index {i} is null.");
                return;
            }

            OutburstAnswersTxts[i].text = items[i];
            OutburstAnswersTxts[i].color = Color.white;
            OutburstAnswersTxts[i].alpha = 0;
        }
    }

    public IEnumerator AnimateTitle()
    {
        TextMeshProUGUI title = OutburstQuestionTitle;
        if (CurrentGame.CurrentRoundType == RoundType.OutburstGeneral) title = OutburstQuestionTitle;
        else if (CurrentGame.CurrentRoundType == RoundType.MondoBurst) title = MondoTitle;

        // Save the final position and scale
        Vector3 finalPos = title.rectTransform.localPosition;
        Vector3 finalScale = title.rectTransform.localScale;

        // Start from center of screen
        Vector3 startPos = Vector3.zero; // localPosition = (0,0,0) in parent canvas
        Vector3 startScale = finalScale * 1.5f; // slightly bigger at start

        title.rectTransform.localPosition = startPos;
        title.rectTransform.localScale = startScale;
        title.alpha = 0;

        float duration = 1f; // total animation time
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // Fade in
            title.alpha = Mathf.Lerp(0, 1, t);

            // Move from center to final position
            title.rectTransform.localPosition = Vector3.Lerp(startPos, finalPos, t);

            // Scale down to final scale
            title.rectTransform.localScale = Vector3.Lerp(startScale, finalScale, t);

            yield return null;
        }

        // Ensure final values
        title.rectTransform.localPosition = finalPos;
        title.rectTransform.localScale = finalScale;
        title.alpha = 1;


        // Countdown
        yield return new WaitForSeconds(1f);
        CountdownObj.SetActive(true);
        CountdownTxt.text = "3";
        yield return new WaitForSeconds(1f);
        CountdownTxt.text = "2";
        yield return new WaitForSeconds(1f);
        CountdownTxt.text = "1";
        yield return new WaitForSeconds(1f);
        CountdownTxt.text = "OUTBURST!";
        yield return new WaitForSeconds(1f);
        CountdownObj.SetActive(false);
    }

    public void OpenMenu()
    {
        MainMenuObj.SetActive(true);
        OutburstGameObj.SetActive(false);
        LeaderboardObj.SetActive(false);
        CurrentState = GameState.Menu;
    }

    public void OpenLeaderboard()
    {
        CurrentState = GameState.Leaderboard;
        LeaderboardObj.SetActive(true);
        MainMenuObj.SetActive(false);
        OutburstGameObj.SetActive(false);
    }

    public bool didGetBonusIndex(HashSet<string> foundAnswers, int index)
    {
        // Safety check in case index is out of bounds
        if (index < 0 || index >= OutburstAnswersTxts.Count)
            return false;

        string answerAtIndex = OutburstAnswersTxts[index].text;

        return foundAnswers.Contains(answerAtIndex);
    }

    public void LoadMondoBurstUI(OutburstQuestion question)
    {
        MondoTitle.text = question.title;
        MondoTitle.alpha = 0;

        for (int i = mondoParent.childCount - 1; i >= 0; i--)
        {
            Destroy(mondoParent.GetChild(i).gameObject);
        }
    }

    public void ShowMondoAnswer(string answer)
    {
        if (mondoPrefab == null || mondoParent == null)
        {
            Debug.LogWarning("Mondo Prefab or Parent not set!");
            return;
        }

        // Create a new TMP text object
        TMP_Text newAnswer = Instantiate(mondoPrefab, mondoParent);

        // Set the text
        newAnswer.text = answer;

        // Random font size
        newAnswer.fontSize = Random.Range(30f, 60f);

        // Random position inside parent rect
        RectTransform parentRect = mondoParent as RectTransform;
        RectTransform rect = newAnswer.rectTransform;

        if (parentRect != null)
        {
            float randX = Random.Range(-parentRect.rect.width / 2f, parentRect.rect.width / 2f);
            float randY = Random.Range(-parentRect.rect.height / 2f, parentRect.rect.height / 2f);

            rect.anchoredPosition = new Vector2(randX, randY);
        }
        else
        {
            // Fallback if mondoParent is not a RectTransform
            rect.localPosition = Random.insideUnitSphere * 100f;
        }
    }
}
