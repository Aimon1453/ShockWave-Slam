using UnityEngine;

/// <summary>
/// 代表一个可以被击飞的物体。
/// </summary>
public interface IKnockable
{
    /// <summary>
    /// 对物体施加一个击飞效果。
    /// </summary>
    /// <param name="knockbackForce">击飞的力（方向和大小）。</param>
    void ApplyKnockback(Vector2 knockbackForce);
}