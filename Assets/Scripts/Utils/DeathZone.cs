using UnityEngine;

// used so player dies if he falls off the map
public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Health playerHealth = other.GetComponent<Health>();
            
            if (playerHealth != null)
            {
                playerHealth.Die();
            }
        }
    }
}