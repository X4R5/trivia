using UnityEngine;

public class AttackArea : Area
{
    [SerializeField] int value = 1;

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }

    protected override void ApplyEffect(GameObject player)
    {
        if (isUpgrade)
        {
            StatsManager.instance.UpgradeDamage(value);
        }
        else
        {
            StatsManager.instance.DowngradeDamage(value);
        }
    }
}
