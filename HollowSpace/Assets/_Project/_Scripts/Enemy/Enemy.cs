using UnityEngine;

[RequireComponent(typeof(Health))]
public class Enemy : MonoBehaviour
{
    public EnemyStats stats;
    private Health _health;

    private void Start()
    {
        _health = GetComponent<Health>();
        _health.SetMaxHealth(stats.maxHealth);
        _health.OnDeath.AddListener(Die);
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    public void TakeDamage(float amount)
    {
        _health.TakeDamage(amount);
        
        // Knockback
        // TODO: Add knockback code here
    }
}