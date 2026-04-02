using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [Header("血量")]
    public Text healthNumberText;
    //闪红
    public Image damageImage;
    public float flashSpeed = 3f;
    public Color flashColor;

    private PlayerHealth playerHealth;
    private bool isFlashing =false;


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


        if (!isDead && currentHealth < maxHealth)
        {
            StartDamageFlash();

        }
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

    public void StartDamageFlash()
    {
        isFlashing = true;
        damageImage.color = flashColor;
    }

    private void Update()
    {
        if (isFlashing && damageImage != null)
        {
            damageImage.color = Color.Lerp(
                damageImage.color,
                new Color(flashColor.r, flashColor.g, flashColor.b, 0),
                flashSpeed * Time.deltaTime
            );
            if (damageImage.color.a <= 0.01f)
            {
                isFlashing = false;
                damageImage.color = new Color(flashColor.r, flashColor.g, flashColor.b, 0);
            }

        }
    }

    // 游戏启动时初始化UI
    private void InitHealthUI()
    {
        healthNumberText.text = $"{playerHealth.health:F0}/{playerHealth.maxHealth:F0}";
        healthNumberText.color = Color.white;

        // 初始化时，把闪红图设为完全透明，隐藏
        if (damageImage != null)
        {
            damageImage.color = new Color(flashColor.r, flashColor.g, flashColor.b, 0);
        }
    }
}