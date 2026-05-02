using UnityEngine;
using UnityEngine.UI;

public class SkillSystem : MonoBehaviour
{
    public static SkillSystem Instance;

    [Header("===== 三个技能 按顺序拖 =====")]
    public Skill[] skills;

    [System.Serializable]
    public class Skill
    {
        public string name;
        public int unlockLevel;

        // 通用冷却（急速充能 / 小男孩用）
        public float cooldown;
        [HideInInspector] public float currentCD;

        // 终极闪光专用：能量系统
        [Header("----- 终极闪光专用 -----")]
        public float maxEnergy = 100f;
        [HideInInspector] public float currentEnergy;
        public float energyCostPerSec = 10f;   // 每秒耗能
        public float energyRecoverPerSec = 15f; // 松开后回能速度
        [HideInInspector] public bool isFiring;

        // UI
        public Image icon;
        public Text text;
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // 初始化终极闪光能量
        foreach (var s in skills)
        {
            if (s.name == "终极闪光")
            {
                s.currentEnergy = s.maxEnergy;
            }
        }
    }

    private void Update()
    {
        UpdateSkillStates();
        CheckInput();
        UpdateAllUI();
    }

    void UpdateSkillStates()
    {
        foreach (var s in skills)
        {
            if (s.name == "急速充能" || s.name == "小男孩")
            {
                // 普通冷却技能
                if (s.currentCD > 0)
                    s.currentCD -= Time.deltaTime;
            }
            else if (s.name == "终极闪光")
            {
                bool unlocked = LevelSystem.Instance.currentLevel >= s.unlockLevel;
                if (!unlocked) continue;

                // 长按 = 消耗能量
                if (s.isFiring)
                {
                    if (s.currentEnergy > 0)
                    {
                        s.currentEnergy -= s.energyCostPerSec * Time.deltaTime;
                        DoUltimateFlashDamage(); // 持续伤害
                    }
                }
                else
                {
                    // 不按 = 回能
                    if (s.currentEnergy < s.maxEnergy)
                        s.currentEnergy += s.energyRecoverPerSec * Time.deltaTime;
                }
            }
        }
    }

    void CheckInput()
    {
        var playerLevel = LevelSystem.Instance.currentLevel;

        // ========== 急速充能 E ==========
        var fastCharge = skills[0];
        if (Input.GetKeyDown(KeyCode.E))
        {
            bool unlocked = playerLevel >= fastCharge.unlockLevel;
            if (unlocked && fastCharge.currentCD <= 0)
            {
                fastCharge.currentCD = fastCharge.cooldown;
                Debug.Log("【急速充能】触发");
            }
        }

        // ========== 终极闪光 长按空格 ==========
        var flash = skills[1];
        bool flashUnlocked = playerLevel >= flash.unlockLevel;

        if (flashUnlocked)
        {
            if (Input.GetKey(KeyCode.Space) && flash.currentEnergy > 0)
            {
                flash.isFiring = true;
            }
            else
            {
                flash.isFiring = false;
            }
        }

        // ========== 小男孩 Q ==========
        var boy = skills[2];
        if (Input.GetKeyDown(KeyCode.Q))
        {
            bool unlocked = playerLevel >= boy.unlockLevel;
            if (unlocked && boy.currentCD <= 0)
            {
                boy.currentCD = boy.cooldown;
                Debug.Log("【小男孩】核弹清屏");
                KillAllEnemies();
            }
        }
    }

    // ========== 技能效果 ==========
    void DoUltimateFlashDamage()
    {
        // 这里写你的持续伤害逻辑
        // 比如：对前方敌人造成持续伤害
        Debug.Log("终极闪光持续伤害中...");
    }

    void KillAllEnemies()
    {
        // 写你的清屏逻辑
        Debug.Log("全图敌人已清空！");
    }

    // ========== UI 更新 ==========
    void UpdateAllUI()
    {
        foreach (var s in skills)
        {
            bool unlocked = LevelSystem.Instance.currentLevel >= s.unlockLevel;

            if (!unlocked)
            {
                s.icon.color = Color.gray;
                s.text.text = "Lv" + s.unlockLevel;
                continue;
            }

            // 普通技能：急速充能 + 小男孩
            if (s.name == "急速充能" || s.name == "小男孩")
            {
                if (s.currentCD > 0)
                {
                    s.icon.color = Color.gray;
                    s.text.text = s.currentCD.ToString("0.0");
                }
                else
                {
                    s.icon.color = Color.white;
                    s.text.text = s.name == "急速充能" ? "E" : "Q";
                }
            }
            // 终极闪光：显示百分比
            else if (s.name == "终极闪光")
            {
                s.icon.color = s.currentEnergy > 0 ? Color.white : Color.gray;
                int percent = Mathf.RoundToInt(s.currentEnergy);
                s.text.text = percent + "%";
            }
        }
    }
}