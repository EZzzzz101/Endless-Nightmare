using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    [Header("面板引用")]
    public GameObject bagPanel;
    public GameObject pausePanel;
    public GameObject gameOverPanel;

    public GameObject hudPanel; 

    public GameObject settingsPanel; 

    public GameObject achievementPanel; 

    //设置按钮
    public Button settingsbtn;
    public static GameUIManager Instance { get; private set; }
    public bool IsAnyPanelOpen { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        CloseAllPanels();
    }

    void Update()
    {
    //     if (Input.GetMouseButtonDown(0))
    // {
    //     var pointerData = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);
    //     pointerData.position = Input.mousePosition;
    //     var results = new System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult>();
    //     UnityEngine.EventSystems.EventSystem.current.RaycastAll(pointerData, results);
    //     foreach (var r in results)
    //         Debug.Log($"命中UI: {r.gameObject.name}");
    // }

        // 暂停状态下，只响应 Esc 恢复
        bool isPaused = pausePanel != null && pausePanel.activeSelf;
        bool isSettingsOpen = settingsPanel != null && settingsPanel.activeSelf;

        //暂停&&设置界面
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 已经暂停面板：恢复整个游戏
            if (isPaused)
            {
                ResumeGamebyPausePanel();
            }

             // 已经在设置界面：恢复整个游戏
            else if (isSettingsOpen)
            {
                ResumeGamebySettingsPanel();
            }

            //未暂停:冻结游戏
            else
            {
                PauseGame();
                //设置面板逻辑
                print("设置界面");
                settingsbtn.onClick.AddListener(ToSettings);
            }
        }
        
        //打开成就系统
        if (Input.GetKeyDown(KeyCode.T) && !isPaused)
        {
            TogglePanel(achievementPanel);           
        }

        
         //打开背包
        if (Input.GetKeyDown(KeyCode.Tab) && !isPaused)
        {
            // 背包：只开关面板，不暂停游戏
            TogglePanel(bagPanel);
        }

    }

    // ========== 背包（不暂停游戏） ==========

    // ========== 暂停游戏 ==========
    public void ToSettings()
    {
        // 这里可以添加设置面板的逻辑
        CloseAllPanels(); 
        ShowPanel(settingsPanel);
    }

  
    void PauseGame()
    {
        CloseAllPanels();           // 先关掉其他面板（比如背包）
        pausePanel?.SetActive(true);

        Time.timeScale = 0;         // 冻结游戏逻辑
        AudioListener.pause = true; // 暂停音频
        IsAnyPanelOpen = true;
    }

    //从暂停界面回到游戏
    void ResumeGamebyPausePanel()
    {
        pausePanel?.SetActive(false);

        Time.timeScale = 1;         // 恢复游戏逻辑
        AudioListener.pause = false;// 恢复音频
        IsAnyPanelOpen = false;     // 或者用 IsAnyActive() 检查
    }

    //从设置界面回到游戏
    void ResumeGamebySettingsPanel()
    {
        settingsPanel?.SetActive(false);

        Time.timeScale = 1;         // 恢复游戏逻辑
        AudioListener.pause = false;// 恢复音频
        IsAnyPanelOpen = false;     // 或者用 IsAnyActive() 检查
    }

    public void GameOver()
    {
        CloseAllPanels();
        //关闭hud
        hudPanel?.SetActive(false);
        ShowPanel(gameOverPanel);
        //延迟显示主菜单
        StartCoroutine(ShowMainMenuAfterGameOverDelay(3f)); // 3秒
    }

    

    // ========== 面板切换 ==========

    void TogglePanel(GameObject panel)
    {
        if (panel != null && panel.activeSelf)
            HidePanel(panel);
        else
            ShowPanel(panel);
    }

    void ShowPanel(GameObject panel)
    {
        if (panel != null)
        {
            panel.SetActive(true);
            IsAnyPanelOpen = true;
        }
    }

    void HidePanel(GameObject panel)
    {
        if (panel != null)
            panel.SetActive(false);
        IsAnyPanelOpen = IsAnyActive();
    }

    void CloseAllPanels()
    {
        if (bagPanel != null)      bagPanel.SetActive(false);
        if (pausePanel != null)    pausePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (achievementPanel != null) achievementPanel.SetActive(false);
        if (hudPanel != null)      hudPanel.SetActive(true); // HUD通常一直显示
        IsAnyPanelOpen = false;
    }

    bool IsAnyActive()
    {
        return (bagPanel != null      && bagPanel.activeSelf)
            || (pausePanel != null    && pausePanel.activeSelf)
            || (gameOverPanel != null && gameOverPanel.activeSelf)
            || (settingsPanel != null && settingsPanel.activeSelf)
            || (achievementPanel != null && achievementPanel.activeSelf);
    }

    public IEnumerator ShowMainMenuAfterGameOverDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        // 这里可以加载主菜单场景，或者显示主菜单面板
        GameManager.Instance.LoadMainScene(); // 替换为你的主菜单场景名称
    }
}
