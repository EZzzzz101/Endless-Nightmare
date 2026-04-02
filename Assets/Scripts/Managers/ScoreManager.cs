using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public delegate void OnScoreChanged(int score);
    public event OnScoreChanged ScoreChanged;

    public int currentScore = 0;

    private void Awake()
    {
        Instance = this;
    }

    // 怪物死亡时调用
    public void AddScore(int killScore)
    {
        Debug.Log("manager接收到分数");
        currentScore += killScore;
        ScoreChanged?.Invoke(currentScore); 
    }
}