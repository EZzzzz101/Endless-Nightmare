using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class EnemyManager : MonoBehaviour
{
    [Header("刷怪设置")]
    public float minSpawnDistance = 15f;
    public float maxSpawnDistance = 25f;
    public float spawnInterval = 3f;
    public int maxSpawnTryCount = 10;

    [Header("3怪概率")]
    public GameObject zomBearPrefab;
    public GameObject zombunnyPrefab;
    public GameObject hellephantPrefab;

    public float chanceZomBear = 45f;
    public float chanceZombunny = 35f;
    public float chanceHellephant = 20f;


    private Transform player;
    private NavMeshHit navHit;

    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        InvokeRepeating(nameof(SpawnEnemy), 0, spawnInterval);
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

            //Debug.Log($"第{i + 1}次尝试刷怪，位置：{randomPos}");

            // 检测这个位置是不是在NavMesh上
            if (NavMesh.SamplePosition(randomPos, out navHit, 2f, NavMesh.AllAreas))
            {
                //Debug.Log($"找到可用刷怪点：{navHit.position}");

                GameObject randomEnemy = GetRandomEnemyByWeight();

                if (randomEnemy != null)
                {
                    GameObject newEnemy = Instantiate(randomEnemy, navHit.position, Quaternion.identity);
                    EnemyBase enemyBase = newEnemy.GetComponent<EnemyBase>();

                    //怪物死亡，分数变化
                    enemyBase.EnemyKilled += ScoreManager.Instance.AddScore;
                }
                return;
            }
        }

        Debug.LogWarning("没找到可用的刷怪点！请扩大刷怪范围，或检查NavMesh是否烘焙正确");
    }
    private GameObject GetRandomEnemyByWeight()
    {
        float totalChance = chanceZomBear + chanceZombunny + chanceHellephant;
        float randomValue = Random.Range(0, totalChance);

        // 先抽 ZomBear
        if (randomValue < chanceZomBear)
        {
            return zomBearPrefab;
        }

        // 再抽 Zombunny
        randomValue -= chanceZomBear;
        if (randomValue < chanceZombunny)
        {
            return zombunnyPrefab; 
        }

        // 最后抽 Hellephant
        return hellephantPrefab;
    }
}