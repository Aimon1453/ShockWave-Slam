using UnityEngine;

/// <summary>
/// 专门处理由多个部分组成（如弹壳+弹头），且需要分离/发射的敌人基类。
/// </summary>
public abstract class MultipartEnemy : EnemyBase
{
    [Header("分体结构引用")]
    [SerializeField] protected GameObject bulletObject; // 弹头
    [SerializeField] protected GameObject caseObject;   // 弹壳

    [Header("通用发射参数")]
    [SerializeField] protected float bulletSpeed = 30f;
    [SerializeField] protected float caseEjectForce = 5f;
    [SerializeField] protected float caseEjectTorque = 20f;

    [Header("特效")]
    [SerializeField] protected GameObject ejectCaseEffect;

    /// <summary>
    /// 通用的抛出弹壳逻辑
    /// </summary>
    protected void EjectCase()
    {
        if (caseObject == null) return;

        if (caseObject.TryGetComponent<Case>(out Case caseScript))
        {
            caseObject.transform.SetParent(null);

            // 2. 激活脚本并初始化
            caseScript.enabled = true;

            // 计算随机方向
            Vector2 ejectDirection = (transform.up - transform.right).normalized;
            float randomTorque = Random.Range(-caseEjectTorque, caseEjectTorque);

            caseScript.Initialize(ejectDirection * caseEjectForce, randomTorque, 1f);
        }
        // 3. 生成抛壳特效
        if (ejectCaseEffect != null)
        {
            Instantiate(ejectCaseEffect, caseObject.transform.position, Quaternion.identity);
        }
    }

    /// <summary>
    /// 通用的发射弹头逻辑
    /// </summary>
    protected void LaunchBullet(Vector2 direction)
    {
        if (bulletObject == null) return;

        // 1. 获取预先挂载的 Bullet 脚本 (KeyBullet 也是 Bullet，所以能获取到)
        if (bulletObject.TryGetComponent<Bullet>(out Bullet bulletScript))
        {
            bulletObject.transform.SetParent(null);

            // 2. 激活脚本并初始化
            bulletScript.enabled = true;
            bulletScript.Initialize(direction, bulletSpeed);
        }
    }
}