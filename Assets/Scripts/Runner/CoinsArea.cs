using UnityEngine;

public class CoinsArea : Area
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
            CoinsManager.instance.AddCoins(value);
        }
        else
        {
            CoinsManager.instance.RemoveCoins(value);
        }
    }
}
