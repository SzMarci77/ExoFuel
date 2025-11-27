using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FadeText : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float fadeTime = 0.5f;

    private Coroutine currentRoutine;

    private void Awake()
    {
        if (text == null)
            text = GetComponent<TextMeshProUGUI>();
    }

    public void ShowText()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(FaderText(1));
    }

    public void HideText()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(FaderText(0));
    }

    IEnumerator FaderText(float targetAlpha)
    {
        float startAlpha = text.alpha;
        float t = 0;

        while (t < fadeTime)
        {
            t += Time.deltaTime;
            float normalized = t / fadeTime;
            text.alpha = Mathf.Lerp(startAlpha, targetAlpha, normalized);
            yield return null;
        }

        text.alpha = targetAlpha;
    }
}
