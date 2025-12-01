using UnityEngine;

public class Jar : GroundEnemyBase
{
    [SerializeField] private GameObject corpsePrefab; // 罐子破碎后的残骸预制体
    [SerializeField] private float corpseLaunchSpeed = 6f; // 尸体初速度
    private bool deathFromKnockback = false;
    protected override void StationaryLogic()
    {
        base.StationaryLogic();
        anim.Play("Jar_Idle");
    }

    protected override void PatrolLogic()
    {
        base.PatrolLogic();
        anim.Play("Jar_Move");
    }

    protected override void SwitchToAirState()
    {
        base.SwitchToAirState();
        anim.Play("Jar_Air");
    }

    protected override void Knockback(Vector2 knockbackDirection, float knockbackForce)
    {
        base.Knockback(knockbackDirection, knockbackForce);
        anim.Play("Jar_Knockback");
    }

    protected override void KnockbackStop(Collision2D collision)
    {
        bool wasKnock = isKnockback;
        base.KnockbackStop(collision);
        currentHealth -= 1;

        if (currentHealth <= 0)
        {
            deathFromKnockback = wasKnock;
            Die();
        }
    }

    protected override void Die()
    {
        var corpse = Instantiate(corpsePrefab, transform.position, transform.rotation);

        // 方向规则：击飞左=>右上；击飞右=>左上；非击飞=>正上
        Vector2 dir = Vector2.up;
        if (deathFromKnockback)
        {
            if (lastKnockbackSign < 0) dir = new Vector2(1f, 1f).normalized;   // 左被击飞
            else if (lastKnockbackSign > 0) dir = new Vector2(-1f, 1f).normalized; // 右被击飞
        }

        if (corpse.TryGetComponent<Rigidbody2D>(out var rb2d))
        {
            rb2d.linearVelocity = dir * corpseLaunchSpeed;
        }
        Destroy(gameObject); // 延迟销毁以播放死亡动画
    }
}