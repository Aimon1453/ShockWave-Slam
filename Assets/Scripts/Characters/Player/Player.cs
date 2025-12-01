using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class Player : MonoBehaviour, IDamagable
{
    [Header("玩家血量")]
    [SerializeField] private int maxHealth = 1; // 最大血量
    private int currentHealth; // 当前血量
    [SerializeField] private Image[] healthImages;
    public Checkpoint CurrentCheckpoint { get; set; }

    [Header("地面检测")]
    [SerializeField] private Transform groundCheck; // 用于检测地面的点
    [SerializeField] private float groundCheckRadius = 0.2f; // 检测范围
    [SerializeField] private LayerMask groundLayer; // 地面的图层

    private PlayerStateMachine stateMachine;
    public Rigidbody2D rb { get; private set; }
    private Animator anim;
    private float originalAnimSpeed;
    PlayerInput input;

    public Vector2 KnockbackForce { get; private set; }//玩家受到伤害时的击退力
    public Vector2 RecoilForce { get; private set; }
    [SerializeField] private bool faceMouse = true; // 是否根据鼠标翻转

    [Header("武器系统")]
    public GameObject weaponObject; // 在编辑器里把子物体 Hammer 拖进去
    public bool HasWeapon = false; // 用于状态机判断是否可以攻击

    [Header("音效")]
    private AudioSource audioSource;


    private void Start()
    {
        // 游戏开始时，强制禁用武器（确保玩家两手空空）
        if (weaponObject != null)
        {
            weaponObject.SetActive(false);
            HasWeapon = false;
        }
    }

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();
        stateMachine = GetComponent<PlayerStateMachine>();
        anim = GetComponentInChildren<Animator>();
        originalAnimSpeed = anim.speed;
        audioSource = GetComponent<AudioSource>();

        currentHealth = maxHealth;
    }

    // 用于检测是否在地面上
    public bool CheckIfGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    public void Move(float speed)
    {
        SetVelocityX(speed * input.AxisX);
    }




    public void FlipTowardsMouse()
    {
        if (!faceMouse) return;
        var cam = Camera.main;
        if (cam == null) return;

        Vector2 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        float dx = mouseWorld.x - transform.position.x;
        if (Mathf.Abs(dx) < 0.01f) return;

        float sign = dx > 0 ? 1f : -1f;
        Vector3 s = transform.localScale;
        if (s.x != sign)
            transform.localScale = new Vector3(sign, s.y, s.z);
    }

    public void SetVelocity(Vector2 velocity)
    {
        rb.linearVelocity = velocity;
    }

    public void SetVelocityX(float velocityX)
    {
        rb.linearVelocity = new Vector2(velocityX, rb.linearVelocity.y);
    }

    public void SetVelocityY(float velocityY)
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, velocityY);
    }

    // 新增：施加一个瞬间的力（用于冲击、反作用力）
    public void AddImpulseForce(Vector2 force)
    {
        rb.AddForce(force, ForceMode2D.Impulse);
    }

    public void SetRecoilForceAndSwitchState(Vector2 force)
    {
        if (stateMachine.currentState is PlayerState_Recoil)
        {
            stateMachine.SwitchState(typeof(PlayerState_Grounded));
        }

        RecoilForce = force;
        stateMachine.SwitchState(typeof(PlayerState_Recoil));
    }

    public void ClearRecoilForce()
    {
        RecoilForce = Vector2.zero;
    }

    // 新增：重置所有速度（用于在空中打击后停止之前的动量）
    public void ResetVelocity()
    {
        rb.linearVelocity = Vector2.zero;
    }

    public void TakeDamage(int damageAmount, Vector2 knockback)
    {
        currentHealth -= damageAmount;

        if (currentHealth <= 0)
        {
            stateMachine.SwitchState(typeof(PlayerState_Hurt));
        }
    }

    public void SetColliderEnabled(bool isEnabled)
    {
        // 假设你用的是 CapsuleCollider2D 或 BoxCollider2D
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = isEnabled;
        }
    }

    public void ClearKnockbackForce()
    {
        KnockbackForce = Vector2.zero;
    }

    public void Respawn()
    {
        if (CurrentCheckpoint != null)
        {
            transform.position = CurrentCheckpoint.GetRespawnPosition();
            rb.linearVelocity = Vector2.zero; // 重置速度

            // 重生后切回地面状态
            stateMachine.SwitchState(typeof(PlayerState_Grounded));
        }
    }

    private void UpdateHealthUI()
    {
        for (int i = 0; i < healthImages.Length; i++)
        {
            if (i < currentHealth)
            {
                healthImages[i].color = Color.green; // 当前血量显示为绿色
            }
            else
            {
                healthImages[i].color = Color.red; // 损失的血量显示为红色
            }
        }
    }

    public void UnlockWeapon()
    {
        if (weaponObject != null)
        {
            stateMachine.SwitchState(typeof(PlayerState_Hold));

            // 可选：播放一个获得武器的音效或特效
        }
    }

    public void TriggerVictoryPose()
    {
        stateMachine.SwitchState(typeof(PlayerState_Victory));
    }
    // 在编辑器中显示地面检测范围，方便调试
    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
