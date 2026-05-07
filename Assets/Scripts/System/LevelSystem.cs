using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class LevelSystem : MonoBehaviour
{
    [Header("基础等级设置")]
    public int currentLevel = 1;
    public float currentExp = 0f;
    public float baseExpToNextLevel = 100f;  // 1级升2级需要100经验
    public float expMultiplier = 1.2f;       // 每级升级经验×1.2，越往后越难升

    [Header("UI引用")]
    public Image expBarFill;                 // 竖版经验条填充Image
    public TMP_Text levelText; // TMP_Text                


    // 计算当前升级所需总经验
    private float ExpToNextLevel => Mathf.Floor(baseExpToNextLevel * Mathf.Pow(expMultiplier, currentLevel - 1));
    //单例模式
    public static LevelSystem Instance;
    private void Awake()
    {
        // 单例：保证场景里只有一个LevelSystem
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        // 初始化UI：显示1级，经验条0%
        UpdateUI();
    }

    /// <summary>
    /// 杀怪后调用这个方法，给玩家加经验
    /// </summary>
    /// <param name="expAmount">杀怪获得的经验值</param>
    public void AddExp(float expAmount)
    {
        currentExp += expAmount;

        // 检查是否可以升级
        while (currentExp >= ExpToNextLevel)
        {
            // 扣除升级所需经验
            currentExp -= ExpToNextLevel;
            // 等级+1
            currentLevel++;
            
            // 成就系统：报告升级了
            AchievementManager.Instance?.ReportLevelUp(currentLevel); 
        }

        // 更新UI
        UpdateUI();
    }

    /// <summary>
    /// 更新经验条和等级数字
    /// </summary>
    public void UpdateUI()
    {
        // 1. 更新经验条填充量（0~1，对应0%~100%）
        float fillAmount = currentExp / ExpToNextLevel;
        expBarFill.fillAmount = fillAmount;

        // 2. 更新等级数字
        levelText.text = currentLevel.ToString();
    }
}