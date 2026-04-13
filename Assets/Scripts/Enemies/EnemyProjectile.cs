using UnityEngine;

public class EnemyProjectile : EnemyDamage
{
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float resetTime;
    private float lifetime;
    public void ActivateProjectile()
    {
        lifetime = 0;
        gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        float movementSpeed = projectileSpeed * Time.deltaTime;
        transform.Translate(movementSpeed, 0, 0);
        
        lifetime += Time.deltaTime;
        if (lifetime > resetTime)
        {
            gameObject.SetActive(false);
        }
    }

    private new void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        gameObject.SetActive(false);
    }
}
