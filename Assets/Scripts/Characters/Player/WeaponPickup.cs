using UnityEngine;
using PrimeTween; // 使用你项目里的 PrimeTween 做个悬浮效果

[RequireComponent(typeof(Collider2D))]
public class WeaponPickup : MonoBehaviour
{
    [Header("动画设置")]
    [SerializeField] private float floatDistance = 0.3f;
    [SerializeField] private float floatDuration = 1f;

    private void Start()
    {
        // 简单的上下浮动动画，让它看起来像个可拾取道具
        Tween.PositionY(transform, transform.position.y + floatDistance, floatDuration, Ease.InOutSine, -1, CycleMode.Yoyo);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检测是否是玩家
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                // 1. 解锁玩家手中的真武器
                player.UnlockWeapon();

                // 2. 销毁地上的假武器
                Destroy(gameObject);
            }
        }
    }
}