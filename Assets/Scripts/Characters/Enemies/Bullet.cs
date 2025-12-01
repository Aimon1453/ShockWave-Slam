using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Bullet : MonoBehaviour
{
    protected Rigidbody2D rb;
    protected Collider2D col;
    protected bool isActive = false; // 用于判断子弹是否处于“激活”的飞行状态

    protected virtual void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    public void Initialize(Vector2 launchDirection, float launchSpeed)
    {
        if (!TryGetComponent<Rigidbody2D>(out rb))
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }

        // 确保碰撞体是触发器，这样它才能穿过物体而不是被弹开
        col.isTrigger = true;
        isActive = true;

        // 设置为无重力，以保证直线飞行
        rb.gravityScale = 0;
        // 设置初始速度
        rb.linearVelocity = launchDirection.normalized * launchSpeed;

        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        // 将弹头的旋转方向对准飞行方向
        transform.rotation = Quaternion.LookRotation(Vector3.forward, launchDirection);
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        // 如果子弹已经不再激活，则不执行任何逻辑
        if (!isActive) return;

        // 检查是否撞到了另一个未发射的子弹
        if (other.TryGetComponent<IShootable>(out IShootable shootable))
        {
            shootable.OnShot(Vector2.zero);
        }

        if (!other.CompareTag("Player"))
        {
            Deactivate();
        }
    }

    protected virtual void Deactivate()
    {
        isActive = false;
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 2f; // 重力稍微大一点，坠落感更强

            // 3. 施加一个随机的弹飞力
            // 向上弹起，并随机向左或向右偏一点
            float randomX = Random.Range(-2f, 2f);
            Vector2 popForce = new Vector2(randomX, 5f); // 5f 是向上的力，可调整

            rb.linearVelocity = popForce;
            rb.angularVelocity = Random.Range(-360f, 360f);
        }

        if (col != null)
        {
            col.enabled = false;
        }

        Destroy(gameObject, 3f);
    }
}