using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject oriPanel;
    public GameObject nextPanel ;
    public GameObject sortPanel;

    //注册界面
    public GameObject NewGamePanel;

    public void Awake()
    {
        ClearPanel();
        oriPanel?.SetActive(true);
    }

    public void OnClickStartGame()
    {
        ClearPanel();
        NewGamePanel?.SetActive(true);

    }

    public void OnClickContinueGame()
    {
        //跳转到存档栏
        ClearPanel();
        nextPanel?.SetActive(true);
        //     // 如果有存档，加载游戏场景
        // if (SaveManager.Instance.HasSaveFile())
        // {
        //     GameManager.Instance.ContinueGame();
        // }
    }



    public void OnClickSortShow()
    {
        ClearPanel();
        sortPanel?.SetActive(true);
    }
    //退出游戏
    public void OnClickQuitGame()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }

    //返回主菜单
     public void OnClickBack()
    {
        ClearPanel();
        oriPanel.SetActive(true);
    }

    //清空mainmenu选项，跳出新界面
    public void ClearPanel()
    {
        oriPanel?.SetActive(false);
        nextPanel?.SetActive(false);
        sortPanel?.SetActive(false);
        NewGamePanel?.SetActive(false);
    }
}
