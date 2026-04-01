using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{
    [Header("刷怪设置")]
    public GameObject enemyPrefab;
    public float minSpawnDistance = 15f;
    public float maxSpawnDistance = 25f;
    public float spawnInterval = 3f;
    public int maxSpawnTryCount = 10;

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

            // 核心：检测这个位置是不是在NavMesh上
            if (NavMesh.SamplePosition(randomPos, out navHit, 2f, NavMesh.AllAreas))
            {
                // 找到可用位置，生成怪物
                Instantiate(enemyPrefab, navHit.position, Quaternion.identity);
                return;
            }
        }

        Debug.LogWarning("没找到可用的刷怪点！请扩大刷怪范围，或检查NavMesh是否烘焙正确");
    }
}