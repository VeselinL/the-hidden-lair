using UnityEngine;

public class EnemyPatrollingScript : EnemyDamage
{
    public Transform pointA;
    public Transform pointB;
    
    public Animator animator;

    public float moveSpeed;
    private Vector3 nextPoint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        if(animator == null) Debug.LogError("Patrolling script doesn't have an Animator Component");

        // Snap to the closest point
        float distA = Vector3.Distance(transform.position, pointA.position);
        float distB = Vector3.Distance(transform.position, pointB.position);
        nextPoint = (distA < distB) ? pointB.position : pointA.position;

        // Snap exactly if starting very close
        if (Vector3.Distance(transform.position, nextPoint) < 0.01f)
            transform.position = nextPoint;
    }

    // Update is called once per frame
    void Update()
    {
        // move to the next point
        transform.position = Vector3.MoveTowards(transform.position, nextPoint, moveSpeed * Time.deltaTime);

        
        if (Vector3.Distance(transform.position, nextPoint) <= 0.01f)
        {
            transform.position = nextPoint; 
            nextPoint = (nextPoint == pointA.position) ? pointB.position : pointA.position; // update next point
        }
        
        if (nextPoint.x > transform.position.x)
            transform.localScale = new Vector3(-1, 1, 1);
        else if(nextPoint.x < transform.position.x)
            transform.localScale = new Vector3(1, 1, 1);
        
        if (animator != null)
            animator.SetBool("moving", true);
    }

}
