using UnityEngine;

public class BoySkill : MonoBehaviour
{
    public static BoySkill Instance;

    public AudioClip boyClip;
    private AudioSource audioSource;

    [Header("技能配置")]
    public float cooldown = 15f;
    public int unlockLevel = 3;
    public int maxCharge = 1;       // 上限1
    public int currentCharge = 1;   // 初始就有1次

    // 状态
    private float currentCD;

    //可用
    public bool useable = true;

    private void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {

        SkillUIManager.Instance.UpdateBoyUI();

        if (Input.GetKeyDown(KeyCode.Q))
            TryUseSkill();
    }

    void TryUseSkill()
    {
        bool unlocked = LevelSystem.Instance.currentLevel >= unlockLevel;
        // 必须解锁 + 有次数
        if (!unlocked || currentCharge <= 0)
            return;

        Debug.Log($"准备播放音效！boyClip是否为空：{boyClip == null}");
        if (boyClip != null)
            audioSource.PlayOneShot(boyClip);
        currentCharge--;
        KillAllEnemies();
        Debug.Log("小男孩核弹已使用！剩余次数：" + currentCharge);
    }

    void KillAllEnemies()
    {
        EnemyBase[] allEnemies = FindObjectsByType<EnemyBase>(FindObjectsSortMode.None);

        foreach (EnemyBase enemy in allEnemies)
        {
            if (!enemy.IsDead)
            {
                enemy.ForceDie();
            }
        }

        Debug.Log($"小男孩核弹爆炸！清空了 {allEnemies.Length} 个敌人！");
    }

    public void AddCharge()
    {
        if (currentCharge < maxCharge)
        {
            currentCharge++;
            Debug.Log("获得小男孩次数！当前：" + currentCharge);
        }
    }

    public int GetCurrentCharge() => currentCharge;
    public bool IsUnlocked() => LevelSystem.Instance.currentLevel >= unlockLevel;
}