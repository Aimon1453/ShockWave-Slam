using UnityEngine;

public class HomingBullet : Bullet, IShootable
{
    [Header("追踪设置")]
    [SerializeField] private float homingSpeed = 1.5f; // 移动速度
    [SerializeField] private float rotateSpeed = 90f; // 旋转速度 (度/秒)，越小滞后感越强，越难转弯

    private Transform playerTransform;
    private bool isHoming = true;

    protected override void Awake()
    {
        base.Awake();
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            // 尝试查找名为 "Center" 的子物体
            Transform centerTransform = player.transform.Find("Center");

            // 如果找到了就用 Center，没找到就用玩家根节点
            playerTransform = centerTransform != null ? centerTransform : player.transform;
        }
    }

    private void Start()
    {
        StartHoming();
    }

    private void StartHoming()
    {
        isHoming = true;
        isActive = true;

        // 确保刚体是 Kinematic，完全由代码控制
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void Update()
    {
        if (isHoming)
        {
            HomingMovement();
        }
    }

    private void HomingMovement()
    {
        if (playerTransform == null) return;

        // 1. 计算目标方向 (玩家位置 - 当前位置)
        Vector2 directionToPlayer = (Vector2)playerTransform.position - (Vector2)transform.position;
        directionToPlayer.Normalize();

        // 2. 计算旋转
        // 计算目标角度 (Z轴旋转)
        // 注意：这里假设你的 Sprite 默认是朝右的。如果默认朝上，需要 -90f
        // 我们可以用 Vector3.Cross 来判断旋转方向，或者直接用 RotateTowards

        float targetAngle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg - 90f; // 假设Sprite朝上

        // 使用 MoveTowardsAngle 平滑旋转当前角度到目标角度
        // rotateSpeed * Time.deltaTime 决定了它一帧能转多少度
        float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, targetAngle, rotateSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        // 3. 移动
        // 始终沿着当前的“上方” (transform.up) 移动
        // 因为我们旋转了物体，transform.up 也会跟着变
        transform.position += transform.up * homingSpeed * Time.deltaTime;
    }

    // 实现 IShootable 接口
    public void OnShot(Vector2 direction)
    {
        if (!isHoming) return;

        isHoming = false;

        // 变身为普通子弹：
        // 1. 恢复 Dynamic 刚体
        // 2. 施加瞬间速度
        base.Initialize(direction, 30f);
    }

    // 碰撞逻辑保持不变...
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (isHoming)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("HomingBullet hit Player");
                //杀死玩家，并销毁自己
                Deactivate();
            }
            // 追踪模式下撞墙通常销毁，或者你可以让它滑行
            else if (!other.CompareTag("Enemy") && other.GetComponent<IShootable>() == null)
            {
                // 撞墙销毁，触发炮台重装填
                Deactivate();
            }
            return;
        }
        base.OnTriggerEnter2D(other);
    }
}