using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [SerializeField] private BossHealth bossHealth; 
    [SerializeField] private Image currentHealthBar;
    [SerializeField] private Image totalHealthBar;
    
    void Update()
    {
        currentHealthBar.fillAmount = bossHealth.currentHealth / bossHealth.startingHealth;
    }
}   