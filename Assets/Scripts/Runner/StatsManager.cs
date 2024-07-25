using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    public static StatsManager instance;

    [SerializeField] int damage = 1;
    [SerializeField] float shootDelay = 0.8f;


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

    public void SetShootDelay(float delay)
    {
        shootDelay = delay;
    }

    public float GetShootDelay()
    {
        return shootDelay;
    }
}
