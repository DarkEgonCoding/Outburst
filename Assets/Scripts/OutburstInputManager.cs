using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using Unity.VisualScripting;

public class OutburstInputManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_InputField mondoInputField;
    [SerializeField] private TextMeshProUGUI timerText;

    [SerializeField] private GameObject AnimationObj;
    [SerializeField] private LetterAnimation letterAnimation;

    public static OutburstInputManager instance;

    private List<string> answers; // The answers for this round
    private HashSet<string> foundAnswers = new HashSet<string>();

    private List<string> exceptions = new List<string> { "THE", "A", "AN", "N", "BAL" };

    private float timer = 60f;
    private bool roundActive = false;

    private Game currGame;
    private int roundScore = 0;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    private void Update()
    {
        if (roundActive)
        {
            if (currGame.CurrentRoundType == RoundType.OutburstGeneral)
            {
                if (!inputField.isFocused)
                {
                    inputField.ActivateInputField();
                }
            }
            else if (currGame.CurrentRoundType == RoundType.MondoBurst)
            {
                if (!mondoInputField.isFocused)
                {
                    mondoInputField.ActivateInputField();
                }
            }
        }
    }

    public void StartMondoBurst(Game game, OutburstQuestion question)
    {
        StartCoroutine(MondoBurstCoroutine(game, question));
    }

    public IEnumerator MondoBurstCoroutine(Game game, OutburstQuestion question)
    {
        AnimationObj.SetActive(true);
        yield return StartCoroutine(letterAnimation.Play("MONDO BURST"));
        AnimationObj.SetActive(false);
        yield return new WaitForSeconds(0.5f);

        List<string> roundAnswers = question.items;

        this.currGame = game;
        roundScore = 0;
        answers = roundAnswers;
        foundAnswers.Clear();

        mondoInputField.text = "";
        mondoInputField.characterLimit = 3;
        mondoInputField.onValueChanged.RemoveAllListeners();

        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(GameController.instance.AnimateTitle());

        mondoInputField.onValueChanged.AddListener(OnMondoInputChanged);
        mondoInputField.Select();

        mondoInputField.interactable = true;
        mondoInputField.ActivateInputField();

        roundActive = true;
        timer = 60f;
        StartCoroutine(TimerCoroutine());
    }

    public void OnMondoInputChanged(string input)
    {
        // Put letters into list
        List<string> letters = new List<string>();
        for (int i = 0; i < input.Length && i < 3; i++)
        {
            if (input[i] != ' ' && input[i] != '\0')
                letters.Add(input[i].ToString());
        }

        GameController.instance.UpdateInputLettersTxt(letters);

        if (!roundActive || string.IsNullOrEmpty(input))
            return;

        input = input.ToUpper(); // make matching case-insensitive
        mondoInputField.text = input;

        foreach (var answer in answers)
        {
            string[] words = answer.Split(' ');

            foreach (var word in words)
            {
                string upperWord = word.ToUpper();

                if (exceptions.Contains(upperWord))
                    continue;

                // Check length-based rules
                if (input.Length < 3)
                {
                    // Only match if input length == word length AND exact match
                    if (input.Length == upperWord.Length && upperWord == input && !foundAnswers.Contains(answer))
                    {
                        Debug.Log($"Matched: {answer} with input {input}");
                        foundAnswers.Add(answer);
                        GameController.instance.ShowMondoAnswer(answer);

                        mondoInputField.text = "";
                        mondoInputField.ActivateInputField();
                    }
                }
                else
                {
                    // Input is 3 letters, match if first 3 letters match
                    if (upperWord.StartsWith(input) && !foundAnswers.Contains(answer))
                    {
                        Debug.Log($"Matched: {answer} with input {input}");
                        foundAnswers.Add(answer);
                        GameController.instance.ShowMondoAnswer(answer);

                        mondoInputField.text = "";
                        mondoInputField.ActivateInputField();
                    }
                }
            }
        }

        if (mondoInputField.text.Length == 3) mondoInputField.text = "";
    }

    public void StartRound(Game game, OutburstQuestion question)
    {
        StartCoroutine(StartRoundCoroutine(game, question));
    }

    public IEnumerator StartRoundCoroutine(Game game, OutburstQuestion question)
    {
        AnimationObj.SetActive(true);
        yield return StartCoroutine(letterAnimation.Play("OUTBURST"));
        AnimationObj.SetActive(false);
        yield return new WaitForSeconds(0.5f);

        List<string> roundAnswers = question.items;

        this.currGame = game;
        roundScore = 0;
        answers = roundAnswers;
        foundAnswers.Clear();

        inputField.text = "";
        inputField.characterLimit = 3;
        inputField.onValueChanged.RemoveAllListeners();

        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(GameController.instance.AnimateTitle());

        inputField.onValueChanged.AddListener(OnInputChanged);
        inputField.Select();

        inputField.interactable = true;
        inputField.ActivateInputField();

        roundActive = true;
        timer = 60f;
        StartCoroutine(TimerCoroutine());
    }

    private void OnInputChanged(string input)
    {
        // Put letters into list
        List<string> letters = new List<string>();
        for (int i = 0; i < input.Length && i < 3; i++)
        {
            if (input[i] != ' ' && input[i] != '\0')
                letters.Add(input[i].ToString());
        }

        GameController.instance.UpdateInputLettersTxt(letters);

        if (!roundActive || string.IsNullOrEmpty(input))
            return;

        input = input.ToUpper(); // make matching case-insensitive
        inputField.text = input;

        foreach (var answer in answers)
        {
            string[] words = answer.Split(' ');

            foreach (var word in words)
            {
                string upperWord = word.ToUpper();

                if (exceptions.Contains(upperWord))
                    continue;

                // Check length-based rules
                if (input.Length < 3)
                {
                    // Only match if input length == word length AND exact match
                    if (input.Length == upperWord.Length && upperWord == input && !foundAnswers.Contains(answer))
                    {
                        Debug.Log($"Matched: {answer} with input {input}");
                        foundAnswers.Add(answer);
                        GameController.instance.ShowAnswer(answer);

                        inputField.text = "";
                        inputField.ActivateInputField();
                    }
                }
                else
                {
                    // Input is 3 letters, match if first 3 letters match
                    if (upperWord.StartsWith(input) && !foundAnswers.Contains(answer))
                    {
                        Debug.Log($"Matched: {answer} with input {input}");
                        foundAnswers.Add(answer);
                        GameController.instance.ShowAnswer(answer);

                        inputField.text = "";
                        inputField.ActivateInputField();
                    }
                }
            }
        }

        if (inputField.text.Length == 3) inputField.text = "";
    }

    private IEnumerator TimerCoroutine()
    {
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            timerText.text = Mathf.CeilToInt(timer).ToString();
            yield return null;
        }

        roundActive = false;
        inputField.interactable = false;
        Debug.Log("Time's up!");
        Debug.Log($"Found {foundAnswers.Count}/{answers.Count} answers");

        int bonusIndex = UnityEngine.Random.Range(0, 10);
        int bonusValue = UnityEngine.Random.Range(1, 4);

        bool didGetBonus = GameController.instance.didGetBonusIndex(foundAnswers, bonusIndex);

        roundScore = foundAnswers.Count;
        if (didGetBonus)
        {
            roundScore += bonusValue;
            Debug.Log($"Bonus! Index {bonusIndex} was correct. +{bonusValue} points.");
        }
        else
        {
            Debug.Log($"No bonus this round. Bonus was on index {bonusIndex} worth {bonusValue}.");
        }

        if(currGame.CurrentRoundType == RoundType.OutburstGeneral) yield return StartCoroutine(GameController.instance.ShowIncompleteAnswers());
        yield return StartCoroutine(GameController.instance.UpdateGameScoreAnimation(currGame, roundScore));

        currGame.StartNextRound();
    }
}
