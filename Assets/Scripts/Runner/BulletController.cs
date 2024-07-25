using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    float lifeTime = 5f, speed = 10f;
    int damage = 1;

    Vector3 direction;

    private void Awake()
    {
        lifeTime = StatsManager.instance.GetBulletLifeTime();
        speed = StatsManager.instance.GetBulletSpeed();
    }

    private void OnEnable()
    {
        Invoke("DestroyBullet", lifeTime);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Debug.Log("Hit enemy");
            collision.GetComponent<EnemyController>().TakeDamage(damage);
            DestroyBullet();
        }
    }

    public void SetDamage(int damage)
    {
        this.damage = damage;
    }

    private void DestroyBullet()
    {
        gameObject.SetActive(false);
    }

    public void SetDirection(Vector3 direction)
    {
        this.direction = direction;
    }
}
