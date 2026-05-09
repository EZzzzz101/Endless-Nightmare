using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private string _currentPlayerId;
    public string CurrentPlayerId => _currentPlayerId;

    // 新游戏：记下 ID → 进 Game
    public void NewGame(string playerId)
    {
        _currentPlayerId = playerId;
        LoadGameScene();
    }

    // 继续游戏：有档才进
    public void ContinueGame(string playerId)
    {
        if (SaveManager.Instance.HasSaveFile(playerId))
        {
            _currentPlayerId = playerId;
            LoadGameScene();
        }
        else
        {
            Debug.LogWarning($"玩家 {playerId} 没有存档");
        }
    }

    void LoadGameScene()
    {
        Time.timeScale = 1;
        AudioListener.pause = false;
        SceneManager.LoadScene("Game");
    }

    public void LoadMainScene()
    {
        Time.timeScale = 1;
        AudioListener.pause = false;
        SceneManager.LoadScene("MainMenu");
    }
}
