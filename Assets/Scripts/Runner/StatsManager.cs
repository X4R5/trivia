using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    public static StatsManager instance;

    [SerializeField] int damage = 1;
    [SerializeField] float shootDelay = 0.8f, bulletLifeTime = 3f, bulletSpeed = 10f;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void SetDamage(int damage)
    {
        this.damage = damage;
    }

    public int GetDamage()
    {
        return damage;
    }

    public void UpgradeDamage(int amount)
    {
        damage += amount;
    }

    public void DowngradeDamage(int amount)
    {
        damage -= amount;

        if (damage < 1)
        {
            damage = 1;
        }
    }

    public void SetShootDelay(float delay)
    {
        shootDelay = delay;
    }

    public float GetShootDelay()
    {
        return shootDelay;
    }

    public void UpgradeShootDelay(float amount)
    {
        shootDelay -= amount;

        if (shootDelay < 0.1f)
        {
            shootDelay = 0.1f;
        }
    }

    public void DowngradeShootDelay(float amount)
    {
        shootDelay += amount;

        if (shootDelay > 2f)
        {
            shootDelay = 2f;
        }
    }

    public void SetBulletLifeTime(float lifeTime)
    {
        bulletLifeTime = lifeTime;
    }

    public float GetBulletLifeTime()
    {
        return bulletLifeTime;
    }

    public void SetBulletSpeed(float speed)
    {
        bulletSpeed = speed;
    }

    public float GetBulletSpeed()
    {
        return bulletSpeed;
    }
}
