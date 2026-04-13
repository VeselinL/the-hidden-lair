using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    public int value = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && CoinManager.Instance != null)
        {
            CoinManager.Instance.AddCoin(value);
            if (AudioManager.instance != null)
                AudioManager.instance.PlaySFX(AudioManager.instance.coinPickup);
            Destroy(gameObject);
        }
    }
}