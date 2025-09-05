using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class LetterAnimation : MonoBehaviour
{
    [SerializeField] private TMP_Text letterPrefab; // assign a TMP_Text prefab
    [SerializeField] private RectTransform container; // parent canvas/container
    [SerializeField] private float animationTime = 5f;
    [SerializeField] private float bounceRadius = 800f;

    private List<Letter> letters = new List<Letter>();
    
    public IEnumerator Play(string word)
    {
        ClearLetters();

        int letterCount = word.Length;
        float spacing = 50f; 
        float startX = -((letterCount - 1) * spacing) / 2f;

        float manualOffsetX = -100f;

        for (int i = 0; i < letterCount; i++)
        {
            TMP_Text letterObj = Instantiate(letterPrefab, container);
            letterObj.text = word[i].ToString();

            Vector2 targetPos = new Vector2(startX + i * spacing + manualOffsetX, 0);

            // Start way off from center (more chaotic)
            Vector2 startPos = targetPos + Random.insideUnitCircle * bounceRadius * 5f;

            letters.Add(new Letter(letterObj.rectTransform, startPos, targetPos));

            // put them at start right away
            letterObj.rectTransform.anchoredPosition = startPos;
        }

        yield return StartCoroutine(AnimateLetters());
    }

    private IEnumerator AnimateLetters()
    {
        float chaosTime = 1.5f; // how long they bounce around
        float timer = 0f;

        // assign random velocities to each letter
        List<Vector2> velocities = new List<Vector2>();
        foreach (Letter l in letters)
        {
            velocities.Add(Random.insideUnitCircle.normalized * Random.Range(300f, 800f));
        }

        // define screen bounds in canvas space
        RectTransform canvasRect = container.root as RectTransform;
        Vector2 screenBounds = canvasRect.rect.size / 2f;

        // chaos phase
        while (timer < chaosTime)
        {
            timer += Time.deltaTime;

            for (int i = 0; i < letters.Count; i++)
            {
                RectTransform rect = letters[i].Rect;
                Vector2 pos = rect.anchoredPosition;
                Vector2 vel = velocities[i];

                // move
                pos += vel * Time.deltaTime;

                // bounce off screen bounds
                if (pos.x < -screenBounds.x || pos.x > screenBounds.x)
                {
                    vel.x *= -1;
                    pos.x = Mathf.Clamp(pos.x, -screenBounds.x, screenBounds.x);
                }
                if (pos.y < -screenBounds.y || pos.y > screenBounds.y)
                {
                    vel.y *= -1;
                    pos.y = Mathf.Clamp(pos.y, -screenBounds.y, screenBounds.y);
                }

                rect.anchoredPosition = pos;
                velocities[i] = vel;
            }

            yield return null;
        }

        // smooth pull into place
        float pullTime = 1f;
        float pullTimer = 0f;

        // capture starting positions before pull
        List<Vector2> startPositions = new List<Vector2>();
        foreach (Letter l in letters)
            startPositions.Add(l.Rect.anchoredPosition);

        while (pullTimer < pullTime)
        {
            pullTimer += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, pullTimer / pullTime);

            for (int i = 0; i < letters.Count; i++)
            {
                letters[i].Rect.anchoredPosition = Vector2.Lerp(
                    startPositions[i],
                    letters[i].TargetPos,
                    t
                );
            }

            yield return null;
        }

        // ensure final alignment
        foreach (Letter l in letters)
            l.Rect.anchoredPosition = l.TargetPos;

        yield return new WaitForSeconds(1f);
    }

    private void ClearLetters()
    {
        foreach (Letter l in letters)
            Destroy(l.Rect.gameObject);

        letters.Clear();
    }

    private class Letter
    {
        public RectTransform Rect;
        public Vector2 StartPos;
        public Vector2 TargetPos;

        public Letter(RectTransform rect, Vector2 startPos, Vector2 targetPos)
        {
            Rect = rect;
            StartPos = startPos;
            TargetPos = targetPos;
        }
    }
}
