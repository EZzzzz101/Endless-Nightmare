using UnityEngine;
using UnityEngine.UI;



public class PlayerHealth : MonoBehaviour
{

    // ====================== 观察者模式======================
    // 1. 定义委托：规定通知要带的参数（当前血量、最大血量、是否死亡）
    public delegate void OnHealthChanged(float currentHealth, float maxHealth, bool isDead);
    // 2. 定义事件：外部只能订阅，不能随便触发
    public event OnHealthChanged HealthChanged;
    //玩家血量
    public float maxHealth = 100;
    public float health;
    //玩家是否死亡
    public bool PlayerIsDeath = false;
    private AudioSource playerAudio;
    private PlayerMovement playerMovement;
    private PlayerShooting playerShooting;
    public AudioClip playerDeathClip;
    private Animator ani;

    private void Awake()
    {
        playerAudio = GetComponent<AudioSource>();
        ani=GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        playerShooting = GetComponentInChildren<PlayerShooting>();
        health = maxHealth;
    }

    //玩家受伤
    public void PlayerTakeDamage(float attackDamage)
    {
        //受击音效
        playerAudio.Play();
        health -= attackDamage;
        //Debug.Log("当前生命值：" + health);

        // ====================== 受伤后发布通知，告诉所有订阅者血量变了 ======================
        HealthChanged?.Invoke(health, maxHealth, PlayerIsDeath);

        //死亡
        if (health <= 0)
        {
            Death();
            PlayerIsDeath = true;
        }
    }

    void Death()
    {
        //死亡音效
        playerAudio.clip = playerDeathClip;
        playerAudio.Play();
        //死亡动画
        ani.SetTrigger("Death");
        //禁止移动，射击
        playerMovement.enabled = false;
        playerShooting.enabled = false;

        // ====================== 死亡时也发布通知 ======================
        HealthChanged?.Invoke(health, maxHealth, PlayerIsDeath);
    }


    // 回血方法
    public void Heal(float healAmount)
    {
        if (PlayerIsDeath) return;
        health += healAmount;
        health = Mathf.Clamp(health, 0, maxHealth);
        HealthChanged?.Invoke(health, maxHealth, PlayerIsDeath);
    }
    public void RestartLevel()
    {

    }
}
