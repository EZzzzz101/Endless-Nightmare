using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [Header("血量")]
    public Text healthNumberText;

    private PlayerHealth playerHealth;

    private void Awake()
    {
        // 自动找到场景里的玩家
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
            // 游戏启动时，先初始化一次血量数字
            InitHealthUI();
        }
        else
        {
            Debug.LogError("没找到玩家！请确认玩家的Tag是Player");
        }
    }

    // 启用时订阅事件
    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.HealthChanged += OnPlayerHealthChanged;
        }
    }

    // 禁用时取消订阅
    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.HealthChanged -= OnPlayerHealthChanged;
        }
    }

    // 收到血量变化通知，更新数字
    private void OnPlayerHealthChanged(float currentHealth, float maxHealth, bool isDead)
    {
        healthNumberText.text = $"{currentHealth:F0}";

        // 玩家死亡时，数字变红
        if (isDead)
        {
            healthNumberText.color = Color.red;
            healthNumberText.text = "0"; // 死亡时强制显示0
        }
        else
        {
            // 非死亡状态恢复白色
            healthNumberText.color = Color.white;
        }
    }

    // 游戏启动时初始化UI
    private void InitHealthUI()
    {
        healthNumberText.text = $"{playerHealth.health:F0}/{playerHealth.maxHealth:F0}";
        healthNumberText.color = Color.white;
    }
}