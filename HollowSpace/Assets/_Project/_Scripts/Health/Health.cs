using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public UnityEvent OnDeath;
    public UnityEvent<float> OnDamageTaken;
    
    [SerializeField] private float maxHealth;
    
    public float MaxHealth => maxHealth;
    public float CurrentHealth { get; private set; }

    private void Start()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        OnDamageTaken?.Invoke(damage);
        
        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
        {
            OnDeath?.Invoke();
        }
    }
    
    public void SetMaxHealth(float health, bool resetHealth = true)
    {
        maxHealth = health;
        if (resetHealth) CurrentHealth = maxHealth;
    }
}