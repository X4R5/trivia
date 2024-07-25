using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 5f;
    public int damage = 1;

    private void Start()
    {
        Invoke("DestroyBullet", lifeTime);
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.CompareTag("Enemy"))
        //{
        //    collision.GetComponent<EnemyController>().TakeDamage(damage);
        //    Destroy(gameObject);
        //}
        
        Debug.Log("Bullet hit: " + collision.name);
        DestroyBullet();
    }

    public void SetDamage(int damage)
    {
        this.damage = damage;
    }

    private void DestroyBullet()
    {
        gameObject.SetActive(false);
    }
}
