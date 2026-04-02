using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
    public Text scoreText;

    // 把Awake改成Start,Start会在所有Awake执行完之后才跑
    void Start()
    {
        
        ScoreManager.Instance.ScoreChanged += UpdateScore;
        scoreText.text = "score: 0";
    }

    void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = $"score: {score}";
        }
    }

    //取消订阅，防止切换场景报错
    void OnDestroy()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ScoreChanged -= UpdateScore;
        }
    }
}