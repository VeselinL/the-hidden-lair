using UnityEngine;
using System.Collections; 

public class SceneEntrance : MonoBehaviour
{
    [SerializeField] private string targetSceneName;
    [SerializeField] private int requiredCoins = 7;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && CoinManager.Instance != null)
        {
            if (CoinManager.Instance.coinCount >= requiredCoins)
            {
                Collider2D col = GetComponent<Collider2D>();
                if (col != null) col.enabled = false;
                
                SceneFader fader = FindFirstObjectByType<SceneFader>(); 
                
                if (fader != null)
                {
                    StartCoroutine(fader.FadeAndLoad(targetSceneName));
                    
                }
                else
                {
                    Debug.LogError("SceneFader not found! Cannot perform smooth transition.");
                    UnityEngine.SceneManagement.SceneManager.LoadScene(targetSceneName);
                }
            }
            else
            {
                Debug.LogWarning($"Coin Requirement: Need {requiredCoins} coins to enter the cave! Player has {CoinManager.Instance.coinCount}.");
            }
        }
    }
}