using UnityEngine;


public class PlayerShooting : MonoBehaviour
{
    public  float timeBetweenBullets = 0.15f;

    private float timer=0;
    private float effectLightDisplayTime = 0.2f;
    private AudioSource gunAudio;
    private Light gunLight;
    public LineRenderer gunLine;
    private ParticleSystem gunParticle;

    //定义ray mask hit
    private Ray shootRay;
    private RaycastHit shootHit;
    private int shootMask;

    //定义武器伤害及倍率
    public int baseDamage;
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
        //先判断是否在激光状态
        if (UltimateFlashSkill.Instance.isFiring)
        {
            Debug.Log("激光发射中");
            // 开启射线
            gunLine.enabled = true;
            gunLine.SetPosition(0, transform.position);

            // 设置颜色 + 粗细
            gunLine.material.color = UltimateFlashSkill.Instance.laserColor;
            gunLine.startWidth = UltimateFlashSkill.Instance.laserWidth;
            gunLine.endWidth = UltimateFlashSkill.Instance.laserWidth;

            // 射线长度
            Ray ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, 100))
            {
                gunLine.SetPosition(1, hit.point);
            }
            else
            {
                gunLine.SetPosition(1, transform.position + transform.forward * 100);
            }

            // 激光期间不显示普通枪口灯光
            gunLight.enabled = false;
        }
        //射击状态
        else
        {
            if (Input.GetMouseButton(0) && timer >= timeBetweenBullets&& !BagManager.IsBagOpen)
            {
                Shooting();
            }

            if (timer >= timeBetweenBullets * effectLightDisplayTime)
            {
                gunLight.enabled = false;
                gunLine.enabled = false;
            }
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

        // 急速充能（E）
        if (FastChargeSkill.Instance != null && FastChargeSkill.Instance.isBuffed)
        {
            gunLine.material.color = Color.red;
            gunLine.startWidth = 0.1f;
            gunLine.endWidth = 0.1f;
        }
        // 普通状态
        else
        {
            gunLine.material.color = Color.yellow;
            gunLine.startWidth = 0.05f;
            gunLine.endWidth = 0.05f;
        }

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


