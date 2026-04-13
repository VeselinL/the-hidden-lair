using UnityEngine;

public class Stone : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damage = 1;              
    public float stunDuration = 3f; 

    [Header("Lifetime Settings")]
    public float destroyAfterSeconds = 5f;  

    private void Start()
    {
        Destroy(gameObject, destroyAfterSeconds);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject hit = collision.gameObject;
        // deal damage when hit
        if (hit.CompareTag("Player"))
        {
            hit.GetComponent<Health>()?.TakeDamage(damage);
        }
        // stun the boss
        else if (hit.CompareTag("Boss"))
        {
            Destroy(gameObject);
            hit.GetComponent<BossHealth>()?.Stun(stunDuration);
        }
        Destroy(gameObject);
    }

}