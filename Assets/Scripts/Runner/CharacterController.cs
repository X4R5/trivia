using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float strafeSpeed = 10f;
    [SerializeField] private float boundary = 5f;

    private Rigidbody rb;
    private Vector2 touchStartPos;
    private Vector2 touchCurrentPos;
    private Vector2 touchDelta;
    private bool isTouching = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        HandleTouchInput();
    }

    void FixedUpdate()
    {
        Vector3 forwardMove = transform.forward * moveSpeed * Time.deltaTime;

        Vector3 strafeMove = new Vector3(touchDelta.x * strafeSpeed * Time.deltaTime, 0, 0);
        Vector3 newPosition = rb.position + forwardMove + strafeMove;

        newPosition.x = Mathf.Clamp(newPosition.x, -boundary, boundary);

        rb.MovePosition(newPosition);
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                touchStartPos = touch.position;
                isTouching = true;
            }
            else if (touch.phase == TouchPhase.Moved && isTouching)
            {
                touchCurrentPos = touch.position;
                touchDelta = touchCurrentPos - touchStartPos;
                touchStartPos = touchCurrentPos;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                touchDelta = Vector2.zero;
                isTouching = false;
            }
        }
        else
        {
            touchDelta = Vector2.zero;
        }
    }
}
