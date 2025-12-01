using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : Singleton<TimeManager>
{
    #region 全局子弹时间 (带豁免)

    public void StartGlobalTimeSlowWithExemption(float scale, float duration, ITimeAffected exemptedObject)
    {
        Time.timeScale = scale;
        Time.fixedDeltaTime = 0.02f * scale; // 调整物理时间步长

        exemptedObject?.SetTimeScale(1.0f);
        StartCoroutine(GlobalTimeSlowWithExemptionCoroutine(scale, duration, exemptedObject));
    }

    private IEnumerator GlobalTimeSlowWithExemptionCoroutine(float scale, float duration, ITimeAffected exemptedObject)
    {
        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = 0.02f; // 恢复默认物理时间步长

        exemptedObject?.SetTimeScale(1.0f);
    }

    /// <summary>
    /// 结束全局子弹时间，并恢复所有对象。
    /// </summary>
    /// <param name="exceptions">之前被豁免的对象列表。</param>
    public void StopGlobalBulletTime(params ITimeAffected[] exceptions)
    {
        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = 0.02f; // 恢复默认物理时间步长

        foreach (var exception in exceptions)
        {
            exception?.SetTimeScale(1.0f);
        }
    }

    #endregion

    #region 局部顿帧

    /// <summary>
    /// 触发一次性的局部顿帧，只影响指定的目标。
    /// </summary>
    /// <param name="targets">要被减速的目标列表。</param>
    /// <param name="scale">减速到的时间尺度 (例如 0.1f)。</param>
    /// <param name="duration">顿帧持续时间 (例如 0.08f)。</param>
    public void TriggerLocalHitStop(List<ITimeAffected> targets, float scale, float duration)
    {
        // 确保列表不为空
        if (targets == null || targets.Count == 0) return;

        StartCoroutine(LocalHitStopCoroutine(targets, scale, duration));
    }

    private IEnumerator LocalHitStopCoroutine(List<ITimeAffected> targets, float scale, float duration)
    {
        // 1. 减速所有指定目标
        foreach (var target in targets)
        {
            // 做一个安全检查，防止目标在顿帧期间被销毁
            if (target != null)
            {
                target.SetTimeScale(scale);
            }
        }

        // 2. 等待指定的持续时间
        // 注意：这里使用常规的 WaitForSeconds，因为我们没有改变全局的 Time.timeScale
        yield return new WaitForSeconds(duration);

        // 3. 恢复所有指定目标的速度
        foreach (var target in targets)
        {
            if (target != null)
            {
                target.SetTimeScale(1.0f);
            }
        }
    }

    public void TriggerGlobalTimeSlow(float scale, float duration)
    {
        StartCoroutine(GlobalTimeSlowCoroutine(scale, duration));
    }

    private IEnumerator GlobalTimeSlowCoroutine(float scale, float duration)
    {
        Time.timeScale = scale;

        // 使用 WaitForSecondsRealtime 确保等待时间不受 Time.timeScale 影响
        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = 1.0f; // 恢复正常时间流速
    }

    #endregion
}