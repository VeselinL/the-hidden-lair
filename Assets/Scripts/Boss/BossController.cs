using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    [Header("Speeds")]
    public float normalSpeed = 5f;
    public float enragedSpeed = 8f;
    private float currentSpeed;

    [Header("Movement")]
    public float stopDistance = 0.5f;

    public Animator anim;
    public bool isStunned;

    [Header("Attack")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private int attackDamage = 2;
    [SerializeField] private float attackCooldown = 2f;
    private float attackTimer = Mathf.Infinity;

    [Header("Stone Spawning")]
    [SerializeField] private StoneSpawn stoneSpawner;
    [SerializeField] private int initialStoneCount = 5;
    [SerializeField] private float initialSpawnInterval = 5f;
    [SerializeField] private float enragedSpawnInterval = 3f;
    private float currentSpawnInterval;
    
    [HideInInspector] public bool canMoveAndAttack;
    private float attackReadyTime;
    private bool isEnraged;

    private float nextStoneTime;


    void Awake()
    {
        // cache components
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }
    }

    void Start()
    {
        StartCoroutine(FindPlayerDelayed());
        StartCoroutine(InitialSpawnDelay(2f));
        currentSpeed = normalSpeed;
        currentSpawnInterval = initialSpawnInterval;
        nextStoneTime = Time.time + currentSpawnInterval;
    }
    private IEnumerator InitialSpawnDelay(float duration)
    {
        yield return new WaitForSeconds(duration); 
        canMoveAndAttack = true;
        attackReadyTime = Time.time + 1f; 
    }
    private IEnumerator FindPlayerDelayed() // used as a bug fix for when the boss chases the player object from the previous scene, after player's death
    {
        yield return null; 
        
        if (PlayerController.Instance != null)
        {
            player = PlayerController.Instance.transform;
        }
        else
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player"); // find the player object in the scene
            if (p != null) 
            {
                player = p.transform;
            }
            else
            {
                Debug.LogError("Boss could not find Player target.");
                player = null;
            }
        }
    }

    void Update()
    {
        if (isStunned || player == null || !canMoveAndAttack) // if stunned boss stays at the same position
        {
            if (rb != null) rb.linearVelocity = Vector2.zero; 
            return;
        }

        attackTimer += Time.deltaTime;
        
        Vector2 target = new Vector2(player.position.x, transform.position.y);
        Vector2 nextPos = Vector2.MoveTowards(transform.position, target, currentSpeed * Time.deltaTime);
        
        transform.position = nextPos;

        FlipToPlayer();
        if (attackTimer >= attackCooldown && Time.time >= attackReadyTime)
        {
            float distance = Vector2.Distance(attackPoint.position, player.position);
            if (distance <= attackRange)
            {
                attackTimer = 0f;
                AttackPlayer();
            }
        }
        if (stoneSpawner != null && Time.time >= nextStoneTime) 
        {
            stoneSpawner.SpawnStones(initialStoneCount);
            nextStoneTime = Time.time + currentSpawnInterval;
        }

    }
    public void ResetAttackReadiness(float delay) 
    {
        attackReadyTime = Time.time + delay;
    }
    private void AttackPlayer()
    {
        if (anim != null) anim.SetTrigger("Attack");

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.enemyAttack);
        }
        
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);
        foreach (Collider2D hit in hitPlayers)
        {
            Health health = hit.GetComponent<Health>();
            if (health != null)
                health.TakeDamage(attackDamage);
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    private void FlipToPlayer() 
    {
        if (sr == null || player == null) return;
        float dir = player.position.x - transform.position.x;
        if (dir > 0.01f) transform.localScale = new Vector3(1f, 1f, 1f);
        else if (dir < -0.01f) transform.localScale = new Vector3(-1f, 1f, 1f);
    }
    
    public void SetEnraged(bool on) // boss runs faster and stones drop quicker
    {
        isEnraged = on;
        currentSpeed = on ? enragedSpeed : normalSpeed;
        currentSpawnInterval =  on ? enragedSpawnInterval : initialSpawnInterval;
        
        if (stoneSpawner != null)
        {
            stoneSpawner.SetEnraged(on);
        }
    }
}