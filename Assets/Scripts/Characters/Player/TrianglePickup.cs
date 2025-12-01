using UnityEngine;
using PrimeTween;

public class TrianglePickup : MonoBehaviour
{
    [SerializeField] private GameObject pickupVFX; // 可选：收集特效

    [Header("浮动动画")]
    [SerializeField] private float floatHeight = 0.3f; // 浮动高度
    [SerializeField] private float cycleDuration = 1f; // 单次浮动时间

    [Header("心跳动画")]
    [SerializeField] private float beatScale = 1.25f; // 心跳时放大的倍数
    [SerializeField] private float beatDuration = 0.15f; // 心跳放大/缩小的时间
    [SerializeField] private float beatInterval = 2f; // 心跳间隔


    private void Start()
    {
        // PrimeTween 的 PositionY 直接传递循环参数
        Tween.PositionY(
            transform,
            transform.position.y + floatHeight,
            cycleDuration,
            cycles: -1,
            cycleMode: CycleMode.Yoyo,
            ease: Ease.InOutSine
        );

        // 启动心跳协程
        StartCoroutine(HeartbeatRoutine());
    }

    private System.Collections.IEnumerator HeartbeatRoutine()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * beatScale;

        while (true)
        {
            // 放大
            Tween.Scale(transform, targetScale, beatDuration, Ease.OutCubic);
            yield return new WaitForSeconds(beatDuration);

            // 缩回
            Tween.Scale(transform, originalScale, beatDuration, Ease.InCubic);
            yield return new WaitForSeconds(beatInterval - beatDuration);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 尝试获取玩家身上的 TriangleCollector 组件
        if (other.TryGetComponent(out TriangleCollector collector))
        {
            collector.CollectTriangle();

            // 生成特效
            if (pickupVFX != null)
            {
                Instantiate(pickupVFX, transform.position, Quaternion.identity);
            }

            // 销毁自身
            Destroy(gameObject);
        }
    }
}