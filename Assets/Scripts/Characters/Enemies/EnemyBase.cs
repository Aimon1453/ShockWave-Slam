using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class EnemyBase : MonoBehaviour
{
    [Header("地面检测")]
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected float groundCheckRadius = 0.2f;
    [SerializeField] protected LayerMask groundLayer;

    protected Rigidbody2D rb;
    private bool isGrounded = false;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Start()
    {
        // 初始状态检查
        isGrounded = CheckIfGrounded();
        if (isGrounded)
        {
            SwitchToIdleState();
        }
        else
        {
            SwitchToAirState();
        }
    }

    protected virtual void Update()
    {
        bool currentlyGrounded = CheckIfGrounded();
        if (currentlyGrounded != isGrounded)
        {
            isGrounded = currentlyGrounded;
            if (isGrounded)
            {
                SwitchToIdleState();
            }
            else
            {
                SwitchToAirState();
            }
        }
    }

    public bool CheckIfGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    // 子类需要实现的具体状态逻辑
    protected abstract void SwitchToIdleState();
    protected abstract void SwitchToAirState();

    protected virtual void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}