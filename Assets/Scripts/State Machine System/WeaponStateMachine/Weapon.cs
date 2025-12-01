using UnityEngine;
using System.Collections.Generic;

public class Weapon : MonoBehaviour, ITimeAffected
{
    [Header("地面冲击波连锁设置")]
    [SerializeField] private GameObject shockwavePrefab; // 冲击波预制体
    [SerializeField] private int shockwaveCount = 5; // 冲击波段数
    [SerializeField] private float shockwaveDistanceStep = 1.5f; // 每段之间的距离
    [SerializeField] private float shockwaveTimeStep = 0.08f; // 每段之间的时间间隔
    [SerializeField] private LayerMask groundLayer; // 地面检测层
    [SerializeField] private float shockwaveYOffset = 0f;

    [Header("空中攻击设置")]
    [SerializeField] private float attackRadius = 3f; // 圆形判定区域的半径
    [SerializeField] private float attackOffsetX = 1f; // 空中攻击的X方向偏移量
    [SerializeField] private LayerMask enemyLayer; // 敌人所在的图层
    [SerializeField] private float knockbackForce = 15f; // 击飞力度
    [SerializeField] private float playerKnockbackForce = 10f; // 玩家反作用力

    private Transform playerTransform; // 玩家位置
    private Vector2 knockbackDirection; // 击飞方向
    private Transform targetEnemy; // 当前选定的目标敌人

    [Header("目标点设置")]
    [SerializeField] private GameObject targetPointPrefab; // 目标点预制体
    [SerializeField, Range(0f, 1f)] private float pointScaleDecreaseStep = 0.04f; // 每个点相对第一个线性递减
    private Vector3 pointBaseScale;
    [Header("敌人路径点设置")]
    [SerializeField] private int maxEnemyTargetPoints = 10; // 最大目标点数量
    [SerializeField] private float pointSpacing = 0.5f; // 目标点之间的间距
    private List<GameObject> enemyTargetPoints = new List<GameObject>(); // 目标点列表
    private Transform enemyCenter;

    [Header("玩家路径点设置")]
    [SerializeField] private int maxPlayerTargetPoints = 10; // 玩家最大目标点数量
    [SerializeField] private float playerPointTimeStep = 0.05f; // 玩家路径点的时间步长
    [SerializeField] private float playerRecoilGravityScale = 1.5f;
    private List<GameObject> playerTargetPoints = new List<GameObject>(); // 玩家目标点列表
    [SerializeField] private Transform playerCenter;

    [Header("顿帧设置")]
    [SerializeField] private float hitStopScale = 0.1f; // 顿帧时的时间尺度
    [SerializeField] private float hitStopDuration = 0.08f; // 顿帧持续时间

    public PlayerStateMachine playerStateMachine;
    private Animator anim;
    private float originalAnimSpeed;

    private void Awake()
    {
        playerStateMachine = GetComponentInParent<PlayerStateMachine>();
        anim = GetComponent<Animator>();
        originalAnimSpeed = anim.speed;
        playerTransform = transform.parent;

        // 初始化敌人目标点
        for (int i = 0; i < maxEnemyTargetPoints; i++)
        {
            GameObject point = Instantiate(targetPointPrefab, transform);
            point.SetActive(false);
            enemyTargetPoints.Add(point);
        }

        // 初始化玩家目标点
        for (int i = 0; i < maxPlayerTargetPoints; i++)
        {
            GameObject point = Instantiate(targetPointPrefab, transform);
            point.SetActive(false);
            playerTargetPoints.Add(point);
        }

        pointBaseScale = targetPointPrefab != null ? targetPointPrefab.transform.localScale : Vector3.one * 0.2f;
    }

    public void DetectTargetEnemy()
    {
        Vector2 attackCenter = (Vector2)playerTransform.position + new Vector2(playerTransform.localScale.x * attackOffsetX, 0);

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackCenter, attackRadius, enemyLayer);

        if (hitEnemies.Length > 0)
        {
            targetEnemy = GetClosestEnemy(hitEnemies);
        }
        else
        {
            targetEnemy = null; // 如果没有敌人，清空目标
        }
    }

    public void PerformAttack()
    {
        if (playerStateMachine.CurrentAttackType == PlayerStateMachine.AttackType.Secondary)
        {
            // 如果是右键攻击，无论在地面还是空中，都执行 AirShockwave
            AirShockwave();
        }
        else  // 否则执行默认逻辑
        {
            if (playerStateMachine.IsGrounded)
            {
                GroundShockwave();
            }
            else
            {
                AirShockwave();
            }
        }
    }

    private void GroundShockwave()
    {
        ShockwaveSegment.ResetHitSet();

        StartCoroutine(SpawnShockwaveChain());
        // Vector2 areaCenter = (Vector2)transform.position + new Vector2(transform.parent.localScale.x * (shockwaveAreaSize.x / 4), 0);
        // Collider2D[] hitTargets = Physics2D.OverlapBoxAll(areaCenter, shockwaveAreaSize, 0f, whatToHit);

        // // 遍历所有被击中的目标
        // foreach (var target in hitTargets)
        // {
        //     if (target.TryGetComponent<Rigidbody2D>(out Rigidbody2D targetRb))
        //     {
        //         targetRb.AddForce(Vector2.up * shockwaveLaunchForce, ForceMode2D.Impulse);
        //     }
        // }

        // TimeManager.Instance.TriggerGlobalTimeSlow(hitStopScale, hitStopDuration);
    }

    private System.Collections.IEnumerator SpawnShockwaveChain()
    {
        Vector2 start = playerTransform.position;
        float direction = Mathf.Sign(playerTransform.localScale.x);

        TimeManager.Instance.TriggerGlobalTimeSlow(hitStopScale, hitStopDuration);

        for (int i = 0; i < shockwaveCount; i++)
        {
            Vector2 spawnPos = start + Vector2.right * direction * shockwaveDistanceStep * i;
            spawnPos.y += shockwaveYOffset;

            // 地面检测，只在有地面处生成
            RaycastHit2D hit = Physics2D.Raycast(spawnPos, Vector2.down, 2f, groundLayer);
            if (hit.collider != null)
            {
                Instantiate(shockwavePrefab, spawnPos, Quaternion.identity);
            }

            yield return new WaitForSeconds(shockwaveTimeStep);
        }
    }

    private void AirShockwave()
    {
        if (targetEnemy != null)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            knockbackDirection = (mousePosition - (Vector2)targetEnemy.position).normalized;

            if (targetEnemy.TryGetComponent<IDamagable>(out IDamagable damageable))
            {
                damageable.TakeDamage(1, knockbackDirection * knockbackForce);
            }
            if (targetEnemy.TryGetComponent<IShootable>(out IShootable shootable))
            {
                shootable.OnShot(knockbackDirection);
            }

            Vector2 recoilDirection = -knockbackDirection;
            playerStateMachine.player.SetRecoilForceAndSwitchState(recoilDirection * playerKnockbackForce);
        }
    }

    #region 玩家目标点绘制
    public void DrawPlayerRecoilTrajectory()
    {
        if (targetEnemy == null)
        {
            ClearPlayerTargetPoints();
            return;
        }

        if (playerStateMachine.IsGrounded && playerStateMachine.CurrentAttackType == PlayerStateMachine.AttackType.Primary)
        {
            ClearPlayerTargetPoints();
            return;
        }

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 knockbackDir = (mousePosition - (Vector2)targetEnemy.position).normalized;
        Vector2 recoilForce = -knockbackDir * playerKnockbackForce;

        Rigidbody2D playerRb = playerStateMachine.player.GetComponent<Rigidbody2D>();

        Vector2 startPos = playerCenter != null ? (Vector2)playerCenter.position : (Vector2)playerTransform.position;

        UpdatePlayerTargetPoints(startPos, recoilForce / playerRb.mass);
    }

    // 简化的路径更新方法
    private void UpdatePlayerTargetPoints(Vector2 startPosition, Vector2 initialVelocity)
    {
        // 使用固定的预测重力
        float gravity = Physics2D.gravity.y * playerRecoilGravityScale;

        for (int i = 0; i < playerTargetPoints.Count; i++)
        {
            float t = i * playerPointTimeStep;

            // 标准抛物线公式
            float x = startPosition.x + initialVelocity.x * t;
            float y = startPosition.y + initialVelocity.y * t + 0.5f * gravity * t * t;

            var p = playerTargetPoints[i];
            p.SetActive(true);
            p.transform.position = new Vector2(x, y);

            // 线性递减：第 i 个点缩放 = 基准 * (1 - 0.03 * i)
            float factor = Mathf.Max(0f, 1f - pointScaleDecreaseStep * i);
            p.transform.localScale = pointBaseScale * factor;
        }
    }

    public void ClearPlayerTargetPoints()
    {
        foreach (var point in playerTargetPoints)
        {
            point.SetActive(false);
        }
    }
    #endregion

    #region 敌人目标点绘制
    public void DrawKnockbackDirection()
    {
        if (targetEnemy == null)
        {
            ClearEnemyTargetPoints();
            return;
        }

        if (playerStateMachine.IsGrounded && playerStateMachine.CurrentAttackType == PlayerStateMachine.AttackType.Primary)
        {
            ClearEnemyTargetPoints();
            // 在这里可以绘制地面冲击波的范围提示（如果需要）
            return;
        }

        if (enemyCenter == null || enemyCenter.parent != targetEnemy)
            enemyCenter = targetEnemy.Find("Center");

        // 获取鼠标位置
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 startPosition = enemyCenter != null ? (Vector2)enemyCenter.position : (Vector2)targetEnemy.position;

        // 计算击飞方向
        knockbackDirection = (mousePosition - startPosition).normalized;

        // 更新目标点位置
        UpdateTargetPoints(startPosition, knockbackDirection);
    }

    private void UpdateTargetPoints(Vector2 startPosition, Vector2 direction)
    {
        for (int i = 0; i < enemyTargetPoints.Count; i++)
        {
            Vector2 pointPosition = startPosition + direction * pointSpacing * i;

            var p = enemyTargetPoints[i];
            p.SetActive(true);
            p.transform.position = pointPosition;

            // 线性递减：第 i 个点缩放 = 基准 * (1 - 0.03 * i)
            float factor = Mathf.Max(0f, 1f - pointScaleDecreaseStep * i);
            p.transform.localScale = pointBaseScale * factor;
        }
    }

    public void ClearEnemyTargetPoints()
    {
        foreach (var point in enemyTargetPoints)
        {
            point.SetActive(false);
        }
    }
    #endregion

    private Transform GetClosestEnemy(Collider2D[] enemies)
    {
        Transform closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (var enemy in enemies)
        {
            Transform enemyCenter = enemy.transform.Find("Center");
            Transform referenceTransform = enemyCenter != null ? enemyCenter : enemy.transform;

            float distance = Vector2.Distance(playerTransform.position, referenceTransform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy.transform; // 返回敌人本体的 Transform
            }
        }

        return closestEnemy;
    }

    public void SetTimeScale(float timeScale)
    {
        anim.speed = originalAnimSpeed * timeScale;
    }

    // private void OnDrawGizmosSelected()
    // {
    //     // 绘制地面冲击波范围
    //     Gizmos.color = Color.red;

    //     float facingDirection = 1f;
    //     if (transform.parent != null)
    //     {
    //         facingDirection = transform.parent.localScale.x;
    //     }

    //     Vector2 areaCenter = (Vector2)transform.position + new Vector2(facingDirection * (shockwaveAreaSize.x / 4), 0);
    //     Gizmos.DrawWireCube(areaCenter, shockwaveAreaSize);


    //     // 绘制空中攻击范围
    //     Gizmos.color = Color.yellow;
    //     Vector2 attackCenter = (Vector2)transform.position + new Vector2(facingDirection * attackOffsetX, 0);
    //     Gizmos.DrawWireSphere(attackCenter, attackRadius);
    // }
}