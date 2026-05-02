using UnityEngine;

public class FastChargeSkill : MonoBehaviour
{
    public static FastChargeSkill Instance;

    public AudioClip chargeClip;
    private AudioSource audioSource;
    private PlayerShooting shooting;

    private int originalDamage;

    [Header("技能配置")]
    public float cooldown = 8f;
    public int unlockLevel = 1;

    [Header("强化效果")]
    public float buffDuration;
    public int buffDamage;
    public bool isBuffed { get; private set; }


    // 内部
    private float currentCD;
    private float buffTimer;

    private void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
        shooting=GetComponentInChildren<PlayerShooting>();
    }

    private void Update()
    {
        // 冷却
        if (currentCD > 0)
            currentCD -= Time.deltaTime;

        // 强化计时
        if (isBuffed)
        {
            buffTimer -= Time.deltaTime;
            if (buffTimer <= 0)
            {
                isBuffed = false;
                Debug.Log("强化结束");
                shooting.baseDamage = originalDamage;
            }
        }

        SkillUIManager.Instance.UpdateFastChargeUI();

        // 输入
        if (Input.GetKeyDown(KeyCode.E))
            TryUseSkill();
    }

    void TryUseSkill()
    {
        bool unlocked = LevelSystem.Instance.currentLevel >= unlockLevel;
        if (!unlocked || currentCD > 0 || isBuffed) return;


        if (chargeClip != null)
            audioSource.PlayOneShot(chargeClip);

        originalDamage=shooting.baseDamage;
        shooting.baseDamage = buffDamage;


        // 放技能
        currentCD = cooldown;
        isBuffed = true;
        buffTimer = buffDuration;

        Debug.Log("E技能 急速充能 激活！");

        // 通知 UI 更新
        SkillUIManager.Instance.UpdateFastChargeUI();
    }

    // 给外部获取状态
    public float GetCD() => currentCD;
    public bool IsUnlocked() => LevelSystem.Instance.currentLevel >= unlockLevel;
}