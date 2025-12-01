using UnityEngine;
using System.Collections;

public class Screw : MonoBehaviour, IShootable
{
    [Header("表现设置")]
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Sprite idleSprite;   // 正常状态图片
    [SerializeField] private Sprite sleepSprite;  // 休眠状态图片

    [Header("逻辑设置")]
    [SerializeField] private float sleepDuration = 5f; // 休眠时长

    [Header("特效")]
    [SerializeField] protected GameObject ejectCaseEffect;

    [Header("顿帧设置")]
    [SerializeField] private float hitStopScale = 0.1f; // 顿帧时的时间尺度
    [SerializeField] private float hitStopDuration = 0.1f; // 顿帧持续时间

    // 记录原始层级，通常是 "Enemy"
    private int originalLayer;
    // 休眠时的层级，通常是 "Default"，确保 Weapon 检测不到
    [SerializeField] private int sleepLayer;

    private bool isSleeping = false;

    private void Awake()
    {
        if (sr == null) sr = GetComponent<SpriteRenderer>();

        // 记录初始层级
        originalLayer = gameObject.layer;
        // 获取 Default 层的 ID (或者你可以指定其他层)
        sleepLayer = LayerMask.NameToLayer("Default");
    }

    public void OnShot(Vector2 direction)
    {
        // 如果正在休眠，不执行任何逻辑
        // (理论上改了 Layer 后 Weapon 根本选不中它，这里是双重保险)
        if (isSleeping) return;

        if (ejectCaseEffect != null)
        {
            Instantiate(ejectCaseEffect, transform.position, Quaternion.identity);
        }

        TimeManager.Instance.TriggerGlobalTimeSlow(hitStopScale, hitStopDuration);

        StartCoroutine(SleepRoutine());
        AudioManager.Instance.PlaySfx(AudioManager.Instance.shockwaveHit); // 播放被击中音效
    }

    private IEnumerator SleepRoutine()
    {
        isSleeping = true;

        // 1. 切换图片
        if (sleepSprite != null) sr.sprite = sleepSprite;

        // 2. 修改 Layer，让 Weapon.cs 的 DetectTargetEnemy 扫描不到它
        // 这样玩家就无法再次对它发动 AirWave，也就没有反冲力了
        gameObject.layer = sleepLayer;

        // 3. 等待倒计时
        yield return new WaitForSeconds(sleepDuration);

        // 4. 恢复状态
        WakeUp();
    }

    private void WakeUp()
    {
        isSleeping = false;

        // 恢复图片
        if (idleSprite != null) sr.sprite = idleSprite;

        // 恢复 Layer，让玩家可以再次互动
        gameObject.layer = originalLayer;
    }
}