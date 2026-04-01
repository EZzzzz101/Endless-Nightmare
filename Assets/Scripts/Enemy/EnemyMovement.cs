using UnityEngine;
using System.Collections;
using UnityEngine.AI;


public class EnemyMovement : MonoBehaviour
{
    private GameObject player;
    private NavMeshAgent nav;
    private EnemyHealth enemyHealth;
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        nav = GetComponent<NavMeshAgent>();
        enemyHealth = GetComponent<EnemyHealth>();
    }

    private void Update()
    {
        if (enemyHealth.death == false)
        nav.SetDestination(player.transform.position);
    }
}
