using UnityEngine;

public class UltimateFlashSkill : MonoBehaviour
{
    public static UltimateFlashSkill Instance;

    [Header("技能配置")]
    public int unlockLevel = 5;
    public float maxEnergy = 100f;
    public float energyCostPerSec;
    public float energyRecoverPerSec;

    [Header("激光效果")]
    public float flashDmgMulti = 2.5f;
    public float laserRange = 100f;
    public float damageInterval = 0.15f;

    [Header("激光外观")]
    public Color laserColor = Color.blue;
    public float laserWidth = 0.15f;

    [Header("音效")]
    public AudioClip laserLoopSound;
    public AudioSource laserAudioSource;

    [Header("重启门槛")]
    [Range(0, 1)] public float restartEnergyPercent = 0.3f;

    public float currentEnergy { get; private set; }
    public bool isFiring { get; private set; }

    private PlayerShooting shooting;
    private float damageTimer;

    private void Awake()
    {
        Instance = this;
        currentEnergy = maxEnergy;
        laserAudioSource.loop = true;
        shooting = GetComponentInChildren<PlayerShooting>();
    }

    private void Update()
    {
        bool unlocked = LevelSystem.Instance.currentLevel >= unlockLevel;
        if (!unlocked)
        {
            isFiring = false;
            StopLaserSound();
            return;
        }

        // 发射逻辑
        if (Input.GetKey(KeyCode.Space))
        {
            if (isFiring && currentEnergy > 0) { }
            else if (!isFiring && currentEnergy >= restartEnergyPercent * maxEnergy)
                isFiring = true;
            else {
                Debug.Log("激光冷却中");
                isFiring = false;
            }
               
        }
        else
            isFiring = false;

        // 开火
        if (isFiring)
            FireLaser();
        else
            StopLaserSound();

        // 能量
        if (isFiring)
            currentEnergy = Mathf.Max(0, currentEnergy - energyCostPerSec * Time.deltaTime);
        else
            currentEnergy = Mathf.Min(maxEnergy, currentEnergy + energyRecoverPerSec * Time.deltaTime);
    }

    void FireLaser()
    {
        if (laserAudioSource == null) return;

        if (laserLoopSound != null && !laserAudioSource.isPlaying)
        {
            laserAudioSource.clip = laserLoopSound;
            laserAudioSource.Play();
        }

        // 伤害间隔
        damageTimer += Time.deltaTime;
        if (damageTimer >= damageInterval)
        {
            damageTimer = 0;
            Ray ray = new Ray(shooting.transform.position, shooting.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, laserRange))
            {
                if (hit.collider.TryGetComponent<EnemyBase>(out var enemy))
                {
                    float dmg = shooting.baseDamage * shooting.dmgMulti * flashDmgMulti;
                    enemy.TakeDamage(dmg, hit.point);
                }
            }
        }
    }
    // 只停声音
    void StopLaserSound()
    {
        if (laserAudioSource != null && laserAudioSource.isPlaying)
            laserAudioSource.Stop();
    }

    public bool IsUnlocked() => LevelSystem.Instance.currentLevel >= unlockLevel;
}