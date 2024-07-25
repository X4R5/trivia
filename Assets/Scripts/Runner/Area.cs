using UnityEngine;

public class Area : MonoBehaviour
{
    public bool isUpgrade;

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ApplyEffect(other.gameObject);
        }
    }

    protected virtual void ApplyEffect(GameObject player)
    {

    }
}
