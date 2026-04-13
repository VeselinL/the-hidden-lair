using UnityEngine;
using System.Collections;

public class BossHealth : MonoBehaviour
{
    public float startingHealth = 10f;
    public float currentHealth { get; private set; }
    private bool dead;
    private bool canTakeDamage;
    [SerializeField] private Animator animator;
    
    [SerializeField] private float enrageMusicPitch = 1.2f; // 20% faster music
    private const float NORMAL_MUSIC_PITCH = 1.0f;
    
    
    [SerializeField] private float invulnerabilityTime = 0.2f;
    private bool isInvulnerable;
    private SpriteRenderer sr;
    private Color originalColor;
    private bool isEnraged;
    [SerializeField] private Color enragedColor = Color.red;
    [SerializeField] private float enrageThreshold = 0.4f; // enrage at 40% health
    [SerializeField] private Collider2D passiveDamageCollider;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            originalColor = sr.color;
    }

    void Start()
    {
        currentHealth = startingHealth;
    }

    public void TakeDamage(float dmg)
    {
        if (!canTakeDamage || dead || isInvulnerable) return; 

        currentHealth = Mathf.Clamp(currentHealth - dmg, 0, startingHealth);
        StartCoroutine(InvulnerabilityCoroutine());
        
        if (!isEnraged && currentHealth <= startingHealth * enrageThreshold)
        {
            SetEnraged(true);
        }

        if (currentHealth <= 0) Die();
    }
    private IEnumerator InvulnerabilityCoroutine()
    {
        isInvulnerable = true;
        
        yield return new WaitForSeconds(invulnerabilityTime);
        
        isInvulnerable = false;
    }
    public void Stun(float duration)
    {
        StartCoroutine(StunCoroutine(duration));
    }

    private IEnumerator StunCoroutine(float duration)
    {
        const float postStunAttackDelay = 2.5f; // boss attack delay post-stun
        canTakeDamage = true;
        if (animator != null) animator.SetBool("Stunned", true);
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.dizzySound, 1.2f);
        }
        
        BossController bc = GetComponent<BossController>(); // get the boss controller script reference
        if (bc != null) bc.isStunned = true;
        if (passiveDamageCollider != null)
        {
            passiveDamageCollider.enabled = false;
        }

        yield return new WaitForSeconds(duration);
        
        canTakeDamage = false;
        if (animator != null) animator.SetBool("Stunned", false);
        if (bc != null) 
        {
            bc.isStunned = false;
            bc.ResetAttackReadiness(postStunAttackDelay);
        }
        if (passiveDamageCollider != null)
        {
            passiveDamageCollider.enabled = true;
        }
    }

    private void Die()
    {
        dead = true;
        if (animator != null) animator.SetTrigger("Death");
        BossController bc = GetComponent<BossController>();
        if (bc != null) bc.enabled = false;
        
        if (passiveDamageCollider != null) passiveDamageCollider.enabled = false;
        if (GameManager.instance != null)
        {
            GameManager.instance.HandleGameWin();
        }
        else
        {
            Debug.LogError("BossHealth needs a GameManager instance to trigger HandleGameWin.");
        }
    }
    // when boss gets under x% hp
    private void SetEnraged(bool on)
    {
        isEnraged = on;
        BossController bc = GetComponent<BossController>();
        if (bc != null) bc.SetEnraged(on);
    
        if (sr != null)
            sr.color = on ? enragedColor : originalColor;
        
        // making the music faster
        if (AudioManager.instance != null)
        {
            float targetPitch = on ? enrageMusicPitch : NORMAL_MUSIC_PITCH;
            AudioManager.instance.SetMusicPitch(targetPitch);
        }
    }
}