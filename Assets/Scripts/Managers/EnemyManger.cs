using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class EnemyManager : MonoBehaviour
{

    public static EnemyManager Instance { get; private set; }

    [Header("刷怪设置")]
    public float minSpawnDistance = 15f;
    public float maxSpawnDistance = 25f;
    public float spawnInterval = 3f;
    public int maxSpawnTryCount = 10;

    [Header("3怪概率")]
    public string tagZomBear = "ZomBear";
    public string tagZombunny = "Zombunny";
    public string tagHellephant = "Hellephant";

    public float chanceZomBear = 45f;
    public float chanceZombunny = 35f;
    public float chanceHellephant = 20f;


    private Transform player;
    private NavMeshHit navHit;

    //是否开始生成
    private bool _isSpawning = false;
    public bool IsSpawning() => _isSpawning;


    void Awake()
    {
        Instance=this;
    }
     void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    
    //开始生成
    public void StartSpawning()
    {
        if (_isSpawning) return;
        _isSpawning = true;
        InvokeRepeating(nameof(SpawnEnemy), 0, spawnInterval);
    }

    //停止生成
    public void StopSpawning()
    {
        _isSpawning = false;
        CancelInvoke(nameof(SpawnEnemy));
    }

    private void SpawnEnemy()
    {
        if (player.GetComponent<PlayerHealth>().PlayerIsDeath)
        {
            CancelInvoke(nameof(SpawnEnemy));
            return;
        }

        // 循环找可用的NavMesh位置
        for (int i = 0; i < maxSpawnTryCount; i++)
        {
            Vector2 randomDir = Random.insideUnitCircle.normalized;
            float randomDistance = Random.Range(minSpawnDistance, maxSpawnDistance);
            Vector3 randomPos = player.position + new Vector3(randomDir.x, 0, randomDir.y) * randomDistance;

            // 检测这个位置是不是在NavMesh上
            if (NavMesh.SamplePosition(randomPos, out navHit, 2f, NavMesh.AllAreas))
            {
                string randomTag = GetRandomEnemyTagByWeight();

                if (!string.IsNullOrEmpty(randomTag))
                {
                    // 从对象池取出怪物
                    GameObject newEnemy = ObjectPool.Instance.GetFromPool(randomTag, navHit.position, Quaternion.identity);

                    if (newEnemy != null)
                    {
                        EnemyBase enemyBase = newEnemy.GetComponent<EnemyBase>();

                        // 手动重置怪物状态
                        enemyBase.ResetEnemy();

                        // 重新订阅死亡事件（因为Reset里清空了）
                        enemyBase.EnemyKilled += ScoreManager.Instance.AddScore;
                    }
                }
                return;
            }
        }
    }
    private string GetRandomEnemyTagByWeight()
    {
        float totalChance = chanceZomBear + chanceZombunny + chanceHellephant;
        float randomValue = Random.Range(0, totalChance);

        if (randomValue < chanceZomBear)
            return tagZomBear;

        randomValue -= chanceZomBear;
        if (randomValue < chanceZombunny)
            return tagZombunny;

        return tagHellephant;
    }

    // EnemyManager 加
    public void ClearAllEnemies()
    {
        CancelInvoke(nameof(SpawnEnemy));
        _isSpawning = false;

        // 把场上的怪全回收
        var enemies = FindObjectsByType<EnemyBase>(FindObjectsSortMode.None);
        foreach (var e in enemies)
        {
            if (!e.IsDead && !string.IsNullOrEmpty(e.poolTag))
                ObjectPool.Instance.ReturnToPool(e.poolTag, e.gameObject);
        }
    }

}