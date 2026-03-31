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

    private void Awake()
    {
        gunAudio = GetComponent<AudioSource>();
        gunLight = GetComponent<Light>();
        gunLine = GetComponent<LineRenderer>();
        gunParticle = GetComponent<ParticleSystem>();
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
        timer = 0;
        
        //启动灯光
        gunLight.enabled = true;
        //启动音效
        gunAudio.Play();
        //发射射线
        gunLine.SetPosition(0, transform.position);
        gunLine.SetPosition(1, transform.position + transform.forward * 100);
        gunLine.enabled = true;
        //启动粒子
        gunParticle.Play();
    }

}


