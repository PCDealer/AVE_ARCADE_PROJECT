using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreSystem : MonoBehaviour
{
    public static float FinalScore;
    public float score;
    public TextMeshProUGUI scoreText;
    private Vector3 originalScale;

    void Start()
    {
        originalScale = scoreText.transform.localScale;
        
        score = 0;
        FinalScore = 0;
    }

    void Update()
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString("N0");
        }
    }

    public IEnumerator PopText()
    {
        scoreText.transform.localScale = originalScale * 1.5f;
        float timer = 0;
        while (timer < 0.1f)
        {
            timer += Time.deltaTime;
            scoreText.transform.localScale = Vector3.Lerp(scoreText.transform.localScale, originalScale, timer / 0.1f);
            yield return null;
        }
        scoreText.transform.localScale = originalScale;
    }

    public void AddScore(float amount)
    {
        score += amount;
        FinalScore = score;
        StopCoroutine(PopText());
        StartCoroutine(PopText());
    }
}