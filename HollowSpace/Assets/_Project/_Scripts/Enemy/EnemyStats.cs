using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Enemy Stats", fileName = "New Enemy Stats")]
public class EnemyStats : ScriptableObject
{
    public int maxHealth;
    public int damage;
    public float speed;
    public float attackRange;
    public float attackCooldown;
}