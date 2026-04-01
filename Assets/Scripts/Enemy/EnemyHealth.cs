using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    public AudioClip deathClip;

    public float health = 100;
    private AudioSource enemyAudio;
    private ParticleSystem enemyParticle;
    private Animator anim;
    public bool death = false;

    private bool isSink = false;

    private void Awake()
    {
        enemyAudio = GetComponent<AudioSource>();
        enemyParticle = GetComponentInChildren<ParticleSystem>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isSink)
        {
            transform.Translate(-transform.up * Time.deltaTime * 1.5f);
        }
    }

    public void TakeDamage(float attackDamage, Vector3 hitPoint)
    {   
        if(death)
            return;
        //受击音效
        enemyAudio.Play();
        enemyParticle.transform.position = hitPoint;
        enemyParticle.Play();
        health -= attackDamage;
        if(health <= 0){
            Death();
        }
    }

    private void Death()
    {
        death = true;
        //播放死亡动画
        anim.SetTrigger("Death");
        StartSinking();
        //销毁寻路
        GetComponent<NavMeshAgent>().enabled = false;
        //碰撞器变为触发器
        GetComponent<CapsuleCollider>().isTrigger = true;
        //刚体变为静态减少开销
        GetComponent<Rigidbody>().isKinematic = true;
        //播放死亡音效
        enemyAudio.clip = deathClip;
        enemyAudio.Play();
    }
    public void StartSinking()
    {
        isSink = true;
        Destroy(gameObject, 2f);
    }
}
