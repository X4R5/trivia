using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootDelayArea : MonoBehaviour
{
    [SerializeField] bool isUpgrade = true;
    [SerializeField] float value = 0.1f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("ShootDelayArea");
            if (isUpgrade)
            {
                StatsManager.instance.UpgradeShootDelay(value);
            }
            else
            {
                StatsManager.instance.DowngradeShootDelay(value);
            }
        }
    }
}
