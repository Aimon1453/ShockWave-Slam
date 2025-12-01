using UnityEngine;

public abstract class GroundEnemyBase : MonoBehaviour, IDamagable
{
    [Header("基础属性")]
    [SerializeField] protected int maxHealth = 2;
    [SerializeField] protected int currentHealth;
    [Header("行为设置")]
    [SerializeField] protected bool canMove = false; // 是否可以巡逻
    [SerializeField] protected float moveSpeed = 2f; // 移动速度

    [Header("巡逻检测")]
    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected Transform ledgeCheck;
    [SerializeField] protected float checkRadius = 0.1f;
    [SerializeField] protected LayerMask whatIsGround;

    [Header("地面检测")]
    [SerializeField] protected Transform groundCheck; // 地面检测点
    [SerializeField] protected float groundCheckRadius = 0.2f; // 检测范围
    [SerializeField] protected LayerMask groundLayer; // 地面的图层

    [Header("对玩家的伤害与击退")]
    [SerializeField] protected float knockbackForce = 10f; // 击退力度
    [SerializeField] protected float knockbackUpwardForce = 5f; // 向上击退力

    // 组件引用
    protected Rigidbody2D rb;
    protected Animator anim;

    // 内部状态
    protected int facingDirection = 1; // 朝向
    protected float originalAnimSpeed;
    protected bool isKnockback = false; // 是否处于击飞状态
    protected int lastKnockbackSign = 0;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        originalAnimSpeed = anim.speed;

        currentHealth = maxHealth;
    }

    protected virtual void Update()
    {
        if (isKnockback) return;

        if (!CheckIfGrounded())
        {
            SwitchToAirState();
            return;
        }

        if (canMove)
        {
            PatrolLogic();
        }
        else
        {
            StationaryLogic();
        }
    }

    protected virtual bool CheckIfGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    protected virtual void PatrolLogic()
    {
        bool isTouchingWall = Physics2D.OverlapCircle(wallCheck.position, checkRadius, whatIsGround);
        bool isNearLedge = !Physics2D.OverlapCircle(ledgeCheck.position, checkRadius, whatIsGround);

        if (isTouchingWall || isNearLedge)
        {
            Flip();
        }

        rb.linearVelocity = new Vector2(moveSpeed * facingDirection, rb.linearVelocity.y);
        // 播放Move动画
    }

    protected virtual void StationaryLogic()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        // 播放Idle动画
    }

    protected virtual void SwitchToAirState()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y);
        // 播放Air动画
    }

    protected virtual void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(facingDirection, 1, 1);
    }

    public virtual void TakeDamage(int damageAmount, Vector2 knockback)
    {
        if (!CheckIfGrounded())
        {
            Hurt(damageAmount);
            Knockback(knockback, knockback.magnitude);
        }
    }

    protected virtual void Hurt(int damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Knockback(Vector2 knockbackDirection, float knockbackForce)
    {
        rb.gravityScale = 0;// 禁用重力
        rb.linearVelocity = Vector2.zero;// 清除当前速度
        rb.AddForce(knockbackDirection.normalized * knockbackForce, ForceMode2D.Impulse);// 添加击飞力
        isKnockback = true; // 进入击飞状态

        lastKnockbackSign = Mathf.Abs(knockbackDirection.x) < 0.01f ? facingDirection : (knockbackDirection.x > 0 ? 1 : -1);
    }

    protected virtual void Die()
    {
        Debug.Log("Enemy Died");
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (isKnockback)
        {
            KnockbackStop(collision);
        }
        else
        {
            if (collision.gameObject.CompareTag("Player") && collision.gameObject.TryGetComponent<IDamagable>(out IDamagable damageable))
            {
                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                if (Mathf.Abs(knockbackDirection.x) < 0.1f)
                {
                    knockbackDirection.x = facingDirection;
                }
                Vector2 force = new Vector2(knockbackDirection.x * knockbackForce, knockbackUpwardForce);

                damageable.TakeDamage(1, force);
            }
        }
    }

    protected virtual void KnockbackStop(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Mechanism"))
        {
            isKnockback = false; // 退出击飞状态
            rb.gravityScale = 2; // 恢复重力
            SwitchToAirState(); // 切换到空中状态

            if (collision.gameObject.CompareTag("Enemy"))
            {
                // 尝试对敌人施加伤害
                if (collision.gameObject.TryGetComponent<IDamagable>(out IDamagable otherEnemy))
                {
                    Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                    otherEnemy.TakeDamage(2, knockbackDirection * knockbackForce);
                }
            }
        }
    }

    protected virtual void OnDrawGizmos()
    {
        if (wallCheck != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(wallCheck.position, checkRadius);
        }
        if (ledgeCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(ledgeCheck.position, checkRadius);
        }
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}