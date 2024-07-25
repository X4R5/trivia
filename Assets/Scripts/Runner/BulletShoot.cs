using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletShoot : MonoBehaviour
{
    [SerializeField] Transform shootPoint;
    float shootDelay;

    private void Start()
    {
        shootDelay = StatsManager.instance.GetShootDelay();
    }

    private void Update()
    {
        shootDelay -= Time.deltaTime;
        if (shootDelay <= 0)
        {
            Shoot();
            shootDelay = StatsManager.instance.GetShootDelay();
        }
    }

    private void Shoot()
    {
        BulletController bullet = ObjectPooler.instance.GetBullet();
        bullet.SetDamage(StatsManager.instance.GetDamage());
        bullet.transform.position = shootPoint.position;
        bullet.SetDirection(Vector3.forward);
        bullet.gameObject.SetActive(true);
    }
}
