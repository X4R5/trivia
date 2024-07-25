using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float followSpeed = 10f;
    Vector3 offset;

    private float initialX;

    private void Start()
    {
        initialX = transform.position.x;
        offset = transform.position - target.position;
    }

    private void FixedUpdate()
    {
        Vector3 targetPosition = target.position + offset;
        targetPosition.x = initialX;

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.fixedDeltaTime);

        transform.position = smoothedPosition;
    }
}
