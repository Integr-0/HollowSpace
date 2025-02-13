using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyPathfind : MonoBehaviour
{
    public Transform target;
    private Enemy _enemy;
    private float _cooldown;
    private Health _targetHealth;

    private void Start()
    {
        _enemy = GetComponent<Enemy>();
        _cooldown = _enemy.stats.attackCooldown;
        _targetHealth = target?.GetComponent<Health>();
    }

    private void Update()
    {
        if (!target) return;
        
        if (IsInAttackRange()) Attack();
        else Pathfind();

        // Cooldown
        _cooldown -= Time.deltaTime;
    }

    private bool IsInAttackRange()
    {
        return (transform.position - target.position).sqrMagnitude <=
               _enemy.stats.attackRange * _enemy.stats.attackRange;
    }

    private void Pathfind()
    {
        // For now, just move towards the target
        // TODO: actual pathfinding, light avoidance
        var direction = (target.position - transform.position).normalized;
        transform.position += _enemy.stats.speed * Time.deltaTime * direction;
    }

    private void Attack()
    {
        if (_cooldown > 0) return;
        
        _cooldown = _enemy.stats.attackCooldown;

        // Deal damage to the target
        if (!_targetHealth)
        {
            Debug.LogWarning($"Tried to attack {target.name}, but it has no Health component.");
            return;
        }

        _targetHealth.TakeDamage(_enemy.stats.damage);
    }
}