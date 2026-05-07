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

    private string SavePath => Path.Combine(Application.persistentDataPath, "save_data.json");

    // 新游戏：删档 → 进 Game
    public void NewGame()
    {
        if (File.Exists(SavePath))
            File.Delete(SavePath);

        LoadGameScene();
    }

    // 继续游戏：有档才进
    public void ContinueGame()
    {
        if (File.Exists(SavePath))
            LoadGameScene();
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
