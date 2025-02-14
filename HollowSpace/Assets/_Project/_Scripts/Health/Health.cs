using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public UnityEvent OnDeath;
    public UnityEvent<float, float> OnDamageTaken; // damage taken, current health
    public UnityEvent<float> OnMaxHealthChanged;
    
    [SerializeField] private float maxHealth;
    
    public float MaxHealth => maxHealth;
    public float CurrentHealth { get; private set; }

    private void Start()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        OnDamageTaken?.Invoke(damage, CurrentHealth);
        if (CurrentHealth <= 0)
        {
            OnDeath?.Invoke();
        }
    }
    
    public void SetMaxHealth(float health, bool resetHealth = true)
    {
        maxHealth = health;
        if (resetHealth) CurrentHealth = maxHealth;
        
        OnMaxHealthChanged?.Invoke(maxHealth);
    }
}