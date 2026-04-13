using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private float playerDamage = 1f;

    [Header("Attack Cooldown")]
    [SerializeField] private float attackCooldown = 0.5f; // seconds between attacks
    private float cooldownTimer = Mathf.Infinity;

    void Update()
    {
        cooldownTimer += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space) && cooldownTimer >= attackCooldown)
        {
            Attack();
            cooldownTimer = 0f;
        }
    }

    void Attack()
    {
        animator.SetTrigger("Attack");
        
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.playerGrunt);
            AudioManager.instance.PlaySFX(AudioManager.instance.playerAttack);
        }
        Collider2D[] attackedEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers); // check the circle for collisions
        foreach (Collider2D enemy in attackedEnemies)
        {
            BossHealth bossHealth = enemy.GetComponent<BossHealth>();
            if (bossHealth != null)
            {
                bossHealth.TakeDamage(playerDamage); 
                continue;
            }

            Health health = enemy.GetComponent<Health>();
            if (health != null)
                health.TakeDamage(playerDamage);
        }

    }

    void OnDrawGizmosSelected() // for visualization of the attack range
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}