using UnityEngine;


public class EnemyAttack : MonoBehaviour
{
    public float attackDamage = 50;
    //在范围内
    private bool playerInRange;
    // 攻击间隔
    public float timeBetweenAttacks = 1f;
    private float timer;
    private PlayerHealth playerHealth;

    private void Update()
    {
        timer += Time.deltaTime;

        // 玩家在范围内 + 到攻击时间 + 玩家没死，攻击
        if (playerInRange && timer >= timeBetweenAttacks && playerHealth != null)
        {
            if (!playerHealth.PlayerIsDeath)
            {
                Attack();
            }
        }
    }

    private void Attack()
    {
        timer = 0f;
        playerHealth.PlayerTakeDamage(attackDamage);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerInRange = true;
            //Debug.Log("Enemy into player");
            //取得玩家血量
            if (other.TryGetComponent<PlayerHealth>(out var health))
            {
                playerHealth = health;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerInRange = false;
            //Debug.Log("Enemy leave player");
        }
    }
}
