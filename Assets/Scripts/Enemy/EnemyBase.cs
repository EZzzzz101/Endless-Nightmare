using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyBase : MonoBehaviour
{
    protected NavMeshAgent agent;       // 寻路组件，对应EnemyMovement
    protected Animator anim;             // 动画组件，对应EnemyHealth的死亡动画
    protected AudioSource enemyAudio;    // 音效组件，对应受伤、死亡音效
    protected ParticleSystem hitParticle;// 受击粒子，对应EnemyHealth的受伤粒子
    protected CapsuleCollider capsuleCollider; // 碰撞体，对应攻击范围、死亡触发
    protected Rigidbody rb;             // 刚体，对应死亡后的物理设置

    protected Transform targetPlayer;    // 玩家的位置，所有怪都要追玩家
    protected PlayerHealth playerHealth; // 玩家的血量脚本，攻击要扣血

    [Header("【怪物基础属性】")]
    public float maxHealth = 100f;       // 最大血量，对应EnemyHealth的health
    public float attackDamage = 50f;      // 攻击力，对应EnemyAttack的attackDamage
    public float timeBetweenAttacks = 1f; // 攻击间隔，对应EnemyAttack的timeBetweenAttacks
    public float moveSpeed = 3.5f;        // 移动速度，对应EnemyMovement的寻路速度
    public AudioClip deathClip;           // 死亡音效，对应EnemyHealth的deathClip


    [Header("【状态变量】不用手动改")]
    protected float currentHealth;    // 当前血量
    protected bool isDead;            // 是否死亡
    protected bool playerInRange;     // 玩家是否在攻击范围内
    protected float attackTimer;      // 攻击计时
    protected bool isSinking;         // 是否正在下沉

    // 定义怪物死亡委托
    public delegate void OnEnemyKilled(int killScore);
    // 定义怪物死亡事件
    public event OnEnemyKilled EnemyKilled;

    [Header("击杀分数")]
    public int killScore = 10;

    //virtual可改
    protected virtual void Awake()
    {
        // 1. 自动获取物体上的所有组件
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        enemyAudio = GetComponent<AudioSource>();
        hitParticle = GetComponentInChildren<ParticleSystem>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();

        // 2. 自动找到场景里的玩家
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            targetPlayer = playerObj.transform;
            playerHealth = playerObj.GetComponent<PlayerHealth>();
        }
        else
        {
            Debug.LogError("没找到玩家！请确认玩家的Tag是Player");
        }

        // 3. 初始化血量
        currentHealth = maxHealth;
    }

    protected virtual void Update()
    {
        // 如果怪已经死了，就不执行任何逻辑
        if (isDead)
        {
            // 死亡后下沉
            if (isSinking)
            {
                transform.Translate(-transform.up * Time.deltaTime * 1.5f);
            }
            return;
        }

        attackTimer += Time.deltaTime;

        if (playerHealth != null && !playerHealth.PlayerIsDeath)
        {
            ChasePlayer(); // 追击玩家
        }
        else
        {
            Idle(); // 玩家死了就待机
        }

        // 通用攻击逻辑
        if (playerInRange && attackTimer >= timeBetweenAttacks)
        {
            Attack();
        }
    }

    // 追击玩家
    protected void ChasePlayer()
    {
        agent.speed = moveSpeed;
        agent.SetDestination(targetPlayer.position);
    }

    // 玩家死了就不动
    protected void Idle()
    {
        agent.speed = 0;
        agent.SetDestination(transform.position);
    }

    //所有怪的基础攻击逻辑
    protected virtual void Attack()
    {
        //玩家死了就不攻击
        if (playerHealth.PlayerIsDeath) return;

        attackTimer = 0f;
        playerHealth.PlayerTakeDamage(attackDamage);
    }

    //所有怪的基础受击逻辑
    public virtual void TakeDamage(float damage, Vector3 hitPoint)
    {
        // 死了就不再受伤
        if (isDead) return;

        // 播放受击音效和粒子
        enemyAudio.Play();
        hitParticle.transform.position = hitPoint;
        hitParticle.Play();

        // 扣血
        currentHealth -= damage;

        // 血量归0就死亡
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // 所有怪的死亡逻辑
    protected virtual void Die()
    {
        isDead = true;

        // 播放死亡动画
        anim.SetTrigger("Death");

        // 关闭寻路、关闭物理
        agent.enabled = false;
        capsuleCollider.isTrigger = true;
        rb.isKinematic = true;

        // 播放死亡音效
        if (deathClip != null)
        {
            enemyAudio.clip = deathClip;
            enemyAudio.Play();
        }
        // 发布被击杀通知
        Debug.Log(gameObject.name + " 死亡"+ killScore+"分");
        EnemyKilled?.Invoke(killScore);

        // 下沉
        StartSinking();
    }

    //下沉销毁
    protected void StartSinking()
    {
        isSinking = true;
        Destroy(gameObject, 2f);
    }

    // 玩家进入攻击范围
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    // 玩家离开攻击范围
    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
