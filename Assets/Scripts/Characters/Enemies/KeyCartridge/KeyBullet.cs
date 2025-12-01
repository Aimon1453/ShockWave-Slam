using UnityEngine;

public class KeyBullet : Bullet
{
    [Header("钥匙属性")]
    [SerializeField] private int keyID = 0;

    [Header("跟随设置")]
    [SerializeField] private float followSpeed = 1.5f;
    [SerializeField] private Vector2 followOffset = new Vector2(0.8f, 0.5f);

    private bool isPickedUp = false;
    private Transform targetPlayer;

    [Header("跟随敌人设置")]
    private bool isFollowingEnemy = false;
    private Transform targetEnemy;
    private float orbitAngle = 0f;
    [SerializeField] private float orbitRadius = 1.2f;
    [SerializeField] private float orbitSpeed = 90f; // 每秒度数

    // 重写碰撞逻辑，处理拾取和开门
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        // 1. 如果已经被拾取，检查是否撞到了门

        if (other.TryGetComponent<IUnlockable>(out IUnlockable unlockable))
        {
            if (unlockable.TryUnlock(keyID))
            {
                Destroy(gameObject); // 开门成功，消耗钥匙
                return;
            }
        }

        // 2. 如果已经被拾取，就不再执行后续的拾取或撞击逻辑了
        if (isPickedUp) return;

        if (!isPickedUp && !isFollowingEnemy && (other.GetComponent<Screw>() || other.GetComponent<Cartridge>()))
        {
            FollowEnemy(other.transform);
            return;
        }

        // 3. 如果还没被拾取，且撞到了玩家 -> 执行拾取
        if (other.CompareTag("Player"))
        {
            PickUp(other.transform);
            return;
        }

        // 4. 如果既没开门，也没被拾取，也不是撞到玩家 -> 执行父类的子弹逻辑 (撞墙、撞敌人)
        // 只有在飞行状态(isActive)下，父类逻辑才会生效
        if (isActive)
        {
            base.OnTriggerEnter2D(other);
        }
    }

    private void Update()
    {
        // 只有被拾取后才执行跟随逻辑
        if (isPickedUp && targetPlayer != null)
        {
            FollowPlayer();
        }
        else if (isFollowingEnemy && targetEnemy != null)
        {
            OrbitEnemy();
        }
    }

    private void PickUp(Transform player)
    {
        if (!TryGetComponent<Rigidbody2D>(out rb))
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }

        if (rb == null) return;

        isActive = false;

        isPickedUp = true;
        isFollowingEnemy = false;
        targetPlayer = player;

        col.isTrigger = true;
        rb.bodyType = RigidbodyType2D.Kinematic; // 彻底关闭物理模拟，完全由代码控制位置
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        AudioManager.Instance.PlaySfx(AudioManager.Instance.keyPickup); // 播放捡起钥匙音效

        // 可选：播放拾取音效
    }

    private void FollowPlayer()
    {
        // 根据玩家朝向计算目标位置
        float facingDir = Mathf.Sign(targetPlayer.localScale.x);
        Vector3 targetPos = targetPlayer.position + new Vector3(followOffset.x * facingDir, followOffset.y, 0);

        // 平滑移动
        transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);

        // 保持钥匙竖直，或者你可以让它旋转
        transform.rotation = Quaternion.identity;
    }

    private void FollowEnemy(Transform enemy)
    {
        isFollowingEnemy = true;
        targetEnemy = enemy;
        // 计算初始角度
        Vector2 toKey = (Vector2)(transform.position - enemy.position);
        orbitAngle = Mathf.Atan2(toKey.y, toKey.x) * Mathf.Rad2Deg;

        AudioManager.Instance.PlaySfx(AudioManager.Instance.keyPickup); // 播放捡起钥匙音效
    }

    private void OrbitEnemy()
    {
        if (targetEnemy == null) return;
        // 顺时针旋转
        orbitAngle -= orbitSpeed * Time.deltaTime;
        float rad = orbitAngle * Mathf.Deg2Rad;
        Vector2 offset = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * orbitRadius;
        transform.position = (Vector2)targetEnemy.position + offset;
        // 保持自身z轴不旋转
        transform.rotation = Quaternion.identity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isPickedUp || isActive) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            PickUp(collision.transform);
        }
    }


    protected override void Deactivate()
    {
        base.Deactivate();
        col.isTrigger = false;
    }
}