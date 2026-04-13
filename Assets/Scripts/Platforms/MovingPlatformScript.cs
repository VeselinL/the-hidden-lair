using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class MovingPlatformScript : MonoBehaviour
{
    public float moveSpeed = 2f;
    public Transform pointA;
    public Transform pointB;
    public Vector2 CurrentVelocity { get; private set; }

    private Vector3 nextPosition;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody2D>();

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    private void Start()
    {
        if (pointA == null || pointB == null)
        {
            enabled = false;
            return;
        }

        nextPosition = pointB.position;
    }

    private void FixedUpdate()
    {
        Vector2 targetPosition = nextPosition;
        Vector2 previousPosition = rb.position;
        Vector2 nextStep = Vector2.MoveTowards(rb.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
        Vector2 movementDelta = nextStep - previousPosition;

        rb.MovePosition(nextStep);
        CurrentVelocity = movementDelta / Time.fixedDeltaTime;

        if (Vector2.Distance(nextStep, targetPosition) <= 0.01f)
        {
            rb.MovePosition(targetPosition);
            CurrentVelocity = (targetPosition - previousPosition) / Time.fixedDeltaTime;

            nextPosition = nextPosition == pointA.position ? pointB.position : pointA.position;
        }
    }

    private void OnDisable()
    {
        CurrentVelocity = Vector2.zero;
    }
}
