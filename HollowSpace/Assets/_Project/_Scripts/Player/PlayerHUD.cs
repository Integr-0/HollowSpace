using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private Health health;
    [Space, SerializeField] private Slider healthSlider;
    [SerializeField] private Slider staminaSlider;
    
    private void Start()
    {
        healthSlider.maxValue = health.MaxHealth;
        healthSlider.value = health.CurrentHealth;
        
        health.OnDamageTaken.AddListener(UpdateHealthSlider);
        health.OnMaxHealthChanged.AddListener(UpdateMaxHealth);
        health.OnDeath.AddListener(OnDeath);
    }
    
    private void UpdateHealthSlider(float damageTaken, float currentHealth)
    {
        healthSlider.value = currentHealth;
    }

    private void UpdateMaxHealth(float newHealth) {
        healthSlider.maxValue = newHealth;
        healthSlider.value = health.CurrentHealth;
    }
    
    public void UpdateStamina(float currentStamina, float maxStamina)
    {
        staminaSlider.maxValue = maxStamina;
        staminaSlider.value = currentStamina;
    }

    private void OnDeath()
    {
        SceneController.Instance.LoadMainMenuAsync();
    }
}
