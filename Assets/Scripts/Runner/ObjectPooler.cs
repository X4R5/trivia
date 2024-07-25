using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler instance;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] int poolSize;
    List<BulletController> bullets = new List<BulletController>();

    private void Awake()
    {
        instance = this;
        for (int i = 0; i < poolSize; i++)
        {
            BulletController bullet = Instantiate(bulletPrefab, transform).GetComponent<BulletController>();
            bullet.gameObject.SetActive(false);
            bullets.Add(bullet);
        }
    }

    public BulletController GetBullet()
    {
        foreach (BulletController bullet in bullets)
        {
            if (!bullet.gameObject.activeInHierarchy)
            {
                return bullet;
            }
        }
        BulletController newBullet = Instantiate(bulletPrefab, transform).GetComponent<BulletController>();
        newBullet.gameObject.SetActive(false);
        bullets.Add(newBullet);
        return newBullet;
    }
}
