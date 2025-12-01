using UnityEngine;

public interface IDamagable
{
    void TakeDamage(int damageAmount, Vector2 knockback);
}