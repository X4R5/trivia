using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    Image healthBar;
    [SerializeField] Transform shootPoint;
    [SerializeField] int maxHealth = 100, prize = 10;
    int currentHealth;
    [SerializeField] float shootDelay;

    float ctr;

    private void Start()
    {
        healthBar = GetComponentInChildren<Image>();
        currentHealth = maxHealth;
        healthBar.fillAmount = 1;
        healthBar.color = Color.green;
        ctr = shootDelay;
    }

    private void Update()
    {
        ctr -= Time.deltaTime;
        if (ctr <= 0)
        {
            Shoot();
            ctr = shootDelay;
        }
    }

    private void Shoot()
    {
        BulletController bullet = ObjectPooler.instance.GetBullet();
        bullet.transform.position = shootPoint.position;
        bullet.SetDirection(Vector3.back);
        bullet.gameObject.SetActive(true);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.fillAmount = (float)currentHealth / maxHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
        else if(currentHealth <= maxHealth / 4)
        {
            healthBar.color = Color.red;
        }
        else if(currentHealth <= maxHealth / 2)
        {
            healthBar.color = Color.yellow;
        }
    }

    private void Die()
    {
        // Add score
        //GameManager.Instance.AddScore(prize);
        // Destroy enemy
        Destroy(gameObject);
    }
}
