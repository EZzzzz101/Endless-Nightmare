using UnityEngine;


public class PlayerShooting : MonoBehaviour
{
    public  float timeBetweenBullets = 0.15f;

    private float timer=0;
    private float effectLightDisplayTime = 0.2f;
    private AudioSource gunAudio;
    private Light gunLight;
    private LineRenderer gunLine;
    private ParticleSystem gunParticle;

    //定义ray mask hit
    private Ray shootRay;
    private RaycastHit shootHit;
    private int shootMask;

    //定义武器伤害及倍率
    public int baseDamage = 50;
    public float dmgMulti = 1f;

    private void Awake()
    {
        gunAudio = GetComponent<AudioSource>();
        gunLight = GetComponent<Light>();
        gunLine = GetComponent<LineRenderer>();
        gunParticle = GetComponent<ParticleSystem>();
        shootMask = LayerMask.GetMask("Shootable");
    }

    void Update()
    {
        timer += Time.deltaTime;
        //逻辑层 
        if (Input.GetMouseButton(0)&&timer>=timeBetweenBullets)
        {
            Shooting();
 
        }

        if (timer >= timeBetweenBullets * effectLightDisplayTime){
            gunLight.enabled = false;
            gunLine.enabled = false;
        }
        //射线(检测位置)
        //表现层(发射特效，击中特效，音效)

    }

    private void Shooting()
    {

        GameEvent.OnGunShot?.Invoke();
        timer = 0;

        //启动灯光
        gunLight.enabled = true;
        //启动音效
        gunAudio.Play();
        //启动粒子
        gunParticle.Play();


        //发射射线
        gunLine.SetPosition(0, transform.position);
        //启动射线开关
        gunLine.enabled = true;
        //射线检测是否命中
        //出发点
        shootRay.origin = transform.position;
        //方向
        shootRay.direction = transform.forward;
        if (Physics.Raycast(shootRay.origin, shootRay.direction, out shootHit, 100, -1)){

            gunLine.SetPosition(1, shootHit.point);
            // 只有打到 Shootable 图层，才执行掉血
            if (shootHit.collider.gameObject.layer == LayerMask.NameToLayer("Shootable"))
            {
                // 安全获取：有EnemyHealth才掉血，没有就跳过
                if (shootHit.collider.TryGetComponent<EnemyBase>(out var enemy))
                {
                    float finalDmg = baseDamage * dmgMulti;
                    enemy.TakeDamage(finalDmg, shootHit.point);
                }
            }
        }
        else gunLine.SetPosition(1, transform.position + transform.forward * 100);

    }

}


