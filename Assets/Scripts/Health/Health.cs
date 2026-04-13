using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Health : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] private float startingHealth;
    [SerializeField] private GameObject coinPrefab;
    private float gravityScale;
    public float currentHealth { get; private set; }

    private bool dead;
    [SerializeField] private float invincibilityDuration = 3f; 
    private bool isInvincible;
    
    [SerializeField] private float deathDuration = 2.5f; 
    [Tooltip("If true, this enemy drops a coin prefab on death. Uncheck for Level 2 enemies.")]
    [SerializeField] private bool dropsCoins = true;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Health animator is null");
        }
        currentHealth = startingHealth;
        dead = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            PlayerController pc = GetComponent<PlayerController>();
            if (pc != null)
                gravityScale = pc.baseGravity; 
            else
                gravityScale = 1f;
        }
    }

    public void TakeDamage(float _damage)
    {
        if (isInvincible || dead) return;

        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);

        if (currentHealth > 0)
        {
            if (animator != null) animator.SetTrigger("Hurt");
            if (AudioManager.instance != null)
                AudioManager.instance.PlaySFX(AudioManager.instance.hurt);

            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.AddForce(Vector2.up * 10f, ForceMode2D.Impulse);

            StartCoroutine(InvincibilityFrames());
        }
        else
        {
            Die();
        }
    }
    
    private IEnumerator InvincibilityFrames()
    {
        if (animator != null) animator.SetTrigger("Hurt");
        isInvincible = true;
        float elapsed = 0f;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        while (elapsed < invincibilityDuration)
        {
            sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(0.1f);
            elapsed += 0.1f;
        }

        sr.enabled = true;
        isInvincible = false;
    }

    public void Die()
    {
        if (dead) return;
        
        if (gameObject.CompareTag("Enemy"))
        {
            Collider2D enemyCollider = GetComponent<Collider2D>();
            if (enemyCollider != null)
                enemyCollider.enabled = false;

            if (animator != null) animator.SetTrigger("Death");
            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlaySFX(AudioManager.instance.enemyDeath);
            }

            EnemyPatrollingScript patrolScript = GetComponent<EnemyPatrollingScript>();
            if (patrolScript != null)
                patrolScript.enabled = false;
            if (dropsCoins && coinPrefab != null)
                Instantiate(coinPrefab, transform.position, Quaternion.identity);

            Destroy(gameObject, 1.5f);
        }
        else if (gameObject.CompareTag("Player"))
        {
            dead = true;
            StartCoroutine(PlayerDeathSequence());
        }
    }

    private IEnumerator PlayerDeathSequence()
    {
        // disable player components
        PlayerController pc = GetComponent<PlayerController>();
        if (pc != null) pc.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
        GetComponent<PlayerCombat>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
        BossController boss = FindFirstObjectByType<BossController>();
        if (boss != null)
        {
            boss.player = null; 
        }
        if (animator != null) animator.SetTrigger("Death");
        if (AudioManager.instance != null)
        {
            AudioManager.instance.StopMusic();
            AudioManager.instance.PlaySFX(AudioManager.instance.playerDeath);
            AudioManager.instance.ResetMusicPitch();
        }
        yield return new WaitForSeconds(deathDuration);
        
        SceneFader fader = FindFirstObjectByType<SceneFader>();
        string currentSceneName = SceneManager.GetActiveScene().name;
        
        if (fader != null)
        {
            yield return fader.FadeAndReload(currentSceneName);
        }
        else
        {
            SceneManager.LoadScene(currentSceneName);
        }
        
        dead = false;
        gameObject.SetActive(false);
    }
}