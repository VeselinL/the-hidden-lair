using UnityEngine;

using UnityEngine.UI;


public class HealthBar : MonoBehaviour

{
    [SerializeField] private Health playerHealth;

    [SerializeField] private Image totalHealthBar;

    [SerializeField] private Image currentHealthBar;

    private void Update()
    {
        currentHealthBar.fillAmount = playerHealth.currentHealth / 10; // hard-coded to 10, there are 10 hearts in the sprite
    }
}