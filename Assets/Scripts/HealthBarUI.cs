using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth = 100;

    public Text healthText;

    private void Start()
    {
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        healthText.text = currentHealth.ToString();
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            // Trigger game over or other necessary actions
        }
        UpdateHealthUI();
    }
}