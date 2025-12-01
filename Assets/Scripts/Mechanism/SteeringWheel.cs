using UnityEngine;
using PrimeTween;

public class SteeringWheel : MonoBehaviour, IShootable
{
    [Header("绑定设置")]
    [SerializeField] private GameObject targetObject; // 拖入 RotateCartridge

    [Header("旋转参数")]
    [SerializeField] private float rotateDuration = 0.3f; // 旋转平滑时间
    [SerializeField] private Ease easeType = Ease.OutBack; // 带一点回弹效果，更有打击感

    private IRotatable targetRotatable;
    private bool isRotating = false;

    private void Start()
    {
        if (targetObject != null)
        {
            targetRotatable = targetObject.GetComponent<IRotatable>();

            // 初始化时，让方向盘的角度与目标保持一致
            if (targetRotatable != null)
            {
                float initialAngle = targetRotatable.GetRotation();
                transform.rotation = Quaternion.Euler(0, 0, initialAngle);
            }
        }
    }

    public void OnShot(Vector2 direction)
    {
        if (isRotating || targetRotatable == null) return;

        // 1. 计算目标角度
        // direction 是玩家冲击波的方向。
        // 我们希望方向盘的“上方”或者某个标记对准这个冲击波的方向，或者顺着冲击波转。
        // 这里假设：方向盘会转动，使其“上方”对齐冲击波的方向（即被冲击波打得指向那边）。

        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        // 减90度是因为 Unity 中 0 度通常指向右边，而我们习惯物体默认朝上。
        // 如果你的美术素材默认朝右，就不需要减 90。

        RotateToAngle(targetAngle);
    }

    private void RotateToAngle(float angle)
    {
        isRotating = true;

        // 1. 旋转方向盘自身
        // 使用 Quaternion.Euler 确保角度是最短路径旋转，或者直接插值角度数值
        Tween.Rotation(transform, Quaternion.Euler(0, 0, angle), rotateDuration, easeType);

        // 2. 旋转绑定的 Cartridge
        // 获取当前角度作为起点
        float startAngle = targetRotatable.GetRotation();

        // 处理角度跨越 360/0 度的问题，使用 Mathf.DeltaAngle 计算最小差值
        float angleDifference = Mathf.DeltaAngle(startAngle, angle);
        float endAngle = startAngle + angleDifference;

        Tween.Custom(startAngle, endAngle, duration: rotateDuration, onValueChange: newVal =>
        {
            targetRotatable.SetRotation(newVal);
        }, ease: easeType)
        .OnComplete(() => isRotating = false);
    }

    private void OnDrawGizmos()
    {
        if (targetObject != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, targetObject.transform.position);
            Gizmos.DrawWireSphere(targetObject.transform.position, 0.5f);
        }
    }
}