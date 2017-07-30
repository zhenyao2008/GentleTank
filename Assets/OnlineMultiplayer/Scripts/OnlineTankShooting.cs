﻿using UnityEngine;
using UnityEngine.UI;

public class OnlineTankShooting : MonoBehaviour
{
    public ObjectPool shellPool;                // 炮弹池
    public OnlineShell onlineShell;             // 同步炮弹
    public Transform shellSpawn;                // 发射子弹的位置
    public Slider aimSlider;                    // 发射时显示黄色箭头
    public AudioSource shootingAudio;           // 当前射击音效
    public AudioClip chargingClip;              // 射击力度距离变化声音
    public AudioClip fireClip;                  // 射击时声音
    public float fireRate = 0.8f;               // 发射间隔
    public float minLaunchForce = 15f;          // 最小发射力度
    public float maxLaunchForce = 30f;          // 最大发射力度
    public float maxChargeTime = 0.75f;         // 最大发射蓄力时间
    public float maxDamage = 100f;              // 最大伤害

    private TankInformation tankInfo;           // 玩家信息
    private float currentLaunchForce;           // 当前发射力度
    private float chargeSpeed;                  // 力度变化速度（最小到最大力度 / 最大蓄力时间）
    private bool fired = true;                  // 是否发射了
    private float nextFireTime;                 // 下一发最早时间

    private string fireButton = "Fire0";        // 发射子弹按钮是名字

    private void Start()
    {
        tankInfo = GetComponent<TankInformation>();
        currentLaunchForce = minLaunchForce;
        aimSlider.value = minLaunchForce;
        chargeSpeed = (maxLaunchForce - minLaunchForce) / maxChargeTime;
    }

    private void Update()
    {
        if (!CanFire())
            return;
        if (!tankInfo.playerAI)         //不是AI才更新
            CheckForceToFire();
    }

    //根据按键时长来发射炮弹
    private void CheckForceToFire()
    {
        // 一直按着到超过最大力度，自动发射
        if (currentLaunchForce >= maxLaunchForce && !fired)
        {
            currentLaunchForce = maxLaunchForce;
            Fire(currentLaunchForce, fireRate, maxDamage);
        }
        // 如果开始按下攻击键，开始射击力度，开始射击变化音效
        else if (Input.GetButtonDown(fireButton))
        {
            fired = false;
            currentLaunchForce = minLaunchForce;

            shootingAudio.clip = chargingClip;
            shootingAudio.Play();

            aimSlider.value = minLaunchForce;
        }
        // 按着攻击键中，增强射击力度
        else if (Input.GetButton(fireButton) && !fired)
        {
            currentLaunchForce += chargeSpeed * Time.deltaTime;
            aimSlider.value = currentLaunchForce;
        }
        // 松开攻击键，Fire In The Hole!!
        else if (Input.GetButtonUp(fireButton) && !fired)
        {
            Fire(currentLaunchForce, fireRate, maxDamage);
        }
    }

    //配置玩家攻击输入属性
    public void SetupPlayerInput(int id)
    {
        if (id < 0)
            return;
        fireButton = "Fire" + id;
    }

    //发射炮弹
    public void Fire(float launchForce, float fireRate, float fireDamage)
    {
        if (!CanFire())
            return;

        //获取炮弹，并发射
        //GameObject shell = shellPool.GetNextObject(true,shellSpawn);
        GameObject shell = PhotonNetwork.Instantiate(onlineShell.name, shellSpawn.position, shellSpawn.rotation, 0);
        shell.GetComponent<Rigidbody>().velocity = launchForce * shellSpawn.forward;
        shell.GetComponent<OnlineShell>().maxDamage = fireDamage;

        shootingAudio.clip = fireClip;
        shootingAudio.Play();

        currentLaunchForce = minLaunchForce;
        aimSlider.value = minLaunchForce;

        nextFireTime = Time.time + fireRate;
    }

    //是否可以攻击
    private bool CanFire()
    {
        if (Time.time > nextFireTime)
            return true;
        return false;
    }

}