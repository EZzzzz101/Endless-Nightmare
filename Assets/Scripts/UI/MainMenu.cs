using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void OnClickStartGame()
    {
        //进入游戏场景
        GameManager.Instance.NewGame();
    }

    public void OnClickContinueGame()
    {
            // 如果有存档，加载游戏场景
        if (SaveManager.Instance.HasSaveFile())
        {
            GameManager.Instance.ContinueGame();
        }
    }
    public void OnClickQuitGame()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }
}
