using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.UIElements;

public class LetterWiggle : MonoBehaviour
{
    [SerializeField] public float maxRotation = 20f;    // Max degrees of rotation
    [SerializeField] public float waitTime = 0.3f;      // Seconds between changes

    [SerializeField] private TextMeshProUGUI tmpText;

    [SerializeField] private GameObject trapezoid;

    public bool RotateNegative;

    private Coroutine coroutine;

    public const int NUM_OF_ROTATIONS = 3;

    private void Start()
    {
        CheckNegativeState();

        coroutine = null;
        coroutine = StartCoroutine(WiggleLoop());
    }

    private void OnDisable()
    {
        // Stop coroutine when object is disabled
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
    }

    private void OnEnable()
    {
        CheckNegativeState();

        // Restart coroutine if it's not running
        if (coroutine == null)
            coroutine = StartCoroutine(WiggleLoop());
    }

    private void CheckNegativeState()
    {
        float zRot = trapezoid.transform.rotation.eulerAngles.z;
        if (zRot > 180f) zRot -= 360f;

        RotateNegative = zRot < 0f;
    }

    private void Update()
    {
        if (coroutine == null)
        {
            coroutine = StartCoroutine(WiggleLoop());
        }
    }

    private IEnumerator WiggleLoop()
    {
        float[] rotations = new float[NUM_OF_ROTATIONS];
        float rotationDivision = maxRotation / NUM_OF_ROTATIONS;

        for (int i = 0; i < NUM_OF_ROTATIONS; i++)
        {
            float upper = maxRotation - (rotationDivision * i);
            float lower = upper - rotationDivision;
            float rot = Random.Range(lower, upper);

            if (RotateNegative) rot *= -1f;

            rotations[i] = rot;
        }

        // Loop through the 3 rotations
        for (int i = 0; i < 3; i++)
        {
            transform.rotation = Quaternion.Euler(0, 0, rotations[i]);
            yield return new WaitForSeconds(waitTime);
        }

        coroutine = null;
    }
}
